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
    }
    public void PauseTimer()
    {
        isRunning = false;
    }
    public void ResumeTimer()
    {
        isRunning = true;
    }
    public void ResetTimer()
    {
        isRunning = false;
        timer = 0f;
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
    #endregion
}
