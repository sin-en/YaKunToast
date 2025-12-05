using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;

public class Timer : MonoBehaviour
{
    public float timer = 0f;
    public bool isRunning = false;
    public TMP_Text timerText;
    public LeaderboardManager leaderboardManager;
    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    private bool hasSubmittedScore = false;
    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    #region Timer Controls
    public void StartTimer()
    {
        isRunning = true;
        timer = 0f;
        hasSubmittedScore = false;
        Debug.Log("Timer started!");
    }
    public void PauseTimer()
    {
        isRunning = false;
        Debug.Log("Timer paused!");
    }
    public void ResumeTimer()
    {
        isRunning = true;
        Debug.Log("Timer resumed!");
    }
    public void ResetTimer()
    {
        isRunning = false;
        timer = 0f;
        hasSubmittedScore = false;
        UpdateTimerDisplay();
        Debug.Log("Timer reset!");
    }
    public void StopTimer()
    {
        if (!isRunning)
        {
            Debug.LogWarning("Timer is not running!");
            return;
        }
        if (hasSubmittedScore)
        {
            Debug.LogWarning("Score has already been submitted!");
            return;
        }
        hasSubmittedScore = true;
        isRunning = false;
        float finalTime = timer;
        
        Debug.Log($"Timer stopped! Final Time: {FormatTime(finalTime)}");

        // Save the completion time
        StartCoroutine(SaveCompletionTime(finalTime));
    }
    #endregion

    #region Save Completion Time
    private IEnumerator SaveCompletionTime(float completionTime)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("No authenticated user found. Cannot save completion time.");
            yield break;
        }
        FirebaseUser user = auth.CurrentUser;
        
        // Create leaderboard entry
        LeaderboardEntry entry = new LeaderboardEntry
        {
            userId = user.UserId,
            userName = user.DisplayName,
            completionTime = completionTime,
            timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        string json = JsonUtility.ToJson(entry);
        
        // Save to Firebase
        var saveTask = dbReference
            .Child("leaderboard")
            .Child(user.UserId)
            .SetRawJsonValueAsync(json);

        yield return new WaitUntil(() => saveTask.IsCompleted);

        if (saveTask.Exception != null)
        {
            Debug.LogError($"Failed to save completion time: {saveTask.Exception}");
        }
        else
        {
            Debug.Log($"Completion time saved successfully: {FormatTime(completionTime)}");
            
            // Submit to leaderboard manager if available
            if (leaderboardManager != null)
            {
                leaderboardManager.SubmitScore(completionTime);
            }
            yield return StartCoroutine(UpdatePlayerCompletionTime(user.UserId, completionTime));
        }
    }

    private IEnumerator UpdatePlayerCompletionTime(string userId, float completionTime)
    {
        var userRef = dbReference.Child("users").Child(userId);
        var getTask = userRef.GetValueAsync();
        yield return new WaitUntil(() => getTask.IsCompleted);

        if (getTask.Exception != null)
        {
            Debug.LogError($"Failed to get user data: {getTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = getTask.Result;
        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            Player playerData = JsonUtility.FromJson<Player>(json);

            // Update time taken
            playerData.timeTaken = completionTime;
            playerData.completedSet = true;
            playerData.completedAt = System.DateTime.UtcNow.ToString("o");

            string updatedJson = JsonUtility.ToJson(playerData);
            var updateTask = userRef.SetRawJsonValueAsync(updatedJson);
            yield return new WaitUntil(() => updateTask.IsCompleted);

            if (updateTask.Exception != null)
            {
                Debug.LogError($"Failed to update player data: {updateTask.Exception}");
            }
            else
            {
                Debug.Log($"Player data updated with completion time!");
            }
        }
    }
    #endregion
    #region UI Display
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(timer);
        }
    }
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        int milliseconds = Mathf.FloorToInt((seconds * 1000f) % 1000f);
        
        return $"{minutes:00}:{secs:00}.{milliseconds:000}";
    }
    public string GetFormattedTime()
    {
        return FormatTime(timer);
    }
    public float GetCurrentTime()
    {
        return timer;
    }
    #endregion
    #region Timer Status
    public bool IsTimerRunning()
    {
        return isRunning;
    }
    public bool HasSubmittedScore()
    {
        return hasSubmittedScore;
    }
    #endregion
}
