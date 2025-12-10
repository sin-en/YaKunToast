/*
* Author: Kwek Sin En
* Date: 26/11/2025
* Description: Handles leaderboard functionalities
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Firebase References")]
    private FirebaseAuth auth;
    private DatabaseReference dbReference;

    [Header("UI References")]
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContainer;

    private int totalUsers = 0;

    #region Unity Lifecycle
    /// <summary>
    /// Initializes Firebase authentication and database references.
    /// </summary>
    private void Awake()
    {
        InitializeFirebase();    
    }
    
    /// <summary>
    /// Initializes Firebase authentication and database references.
    /// </summary>
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        Debug.Log("LeaderboardManager initialized");
    }
    #endregion

    #region Submit Score
    /// <summary>
    /// Submits the player's score to the leaderboard.
    /// </summary>
    /// <param name="completionTime"></param>
    public void SubmitScore(float completionTime)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("Cannot submit score - user not logged in!");
            return;
        }
        StartCoroutine(SubmitScoreCoroutine(auth.CurrentUser.UserId, auth.CurrentUser.DisplayName, completionTime));
    }

    /// <summary>
    /// Coroutine to submit score to Firebase Realtime Database.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <param name="completionTime"></param>
    /// <returns></returns>
    private IEnumerator SubmitScoreCoroutine(string userId, string userName, float completionTime)
    {
        // Create leaderboard entry
        var leaderboardEntry = new LeaderboardEntry
        {
            userId = userId,
            userName = userName,
            completionTime = completionTime,
            timestamp = GetCurrentTimestamp()
        };

        string json = JsonUtility.ToJson(leaderboardEntry);
        
        var submitTask = dbReference.Child("leaderboard").Child(userId).SetRawJsonValueAsync(json);
        yield return new WaitUntil(() => submitTask.IsCompleted);

        if (submitTask.Exception != null)
        {
            Debug.LogError($"Failed to submit score: {submitTask.Exception}");
        }
        else
        {
            Debug.Log($"Score submitted successfully for {userName}: {completionTime}s");
            yield return StartCoroutine(UpdateUserBestTime(userId, completionTime));
        }
    }

    /// <summary>
    /// Updates the user's best time if the new time is better.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="newTime"></param>
    /// <returns></returns>
    private IEnumerator UpdateUserBestTime(string userId, float newTime)
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

            // Update if this is a better time (lower is better) or first time
            if (playerData.timeTaken == 0 || newTime < playerData.timeTaken)
            {
                playerData.timeTaken = newTime;
                string updatedJson = JsonUtility.ToJson(playerData);
                
                var updateTask = userRef.SetRawJsonValueAsync(updatedJson);
                yield return new WaitUntil(() => updateTask.IsCompleted);

                if (updateTask.Exception != null)
                {
                    Debug.LogError($"Failed to update user best time: {updateTask.Exception}");
                }
                else
                {
                    Debug.Log($"Updated best time for user {userId}: {newTime}s");
                }
            }
        }
    }
    #endregion

    #region Fetch and Display Leaderboard
    /// <summary>
    /// Fetches and displays leaderboard entries.
    /// </summary>
    public void ShowLeaderboard()
    {
        StartCoroutine(FetchLeaderboardData());
    }

    /// <summary>
    /// Coroutine to fetch leaderboard data from Firebase Realtime Database.
    /// </summary>
    private IEnumerator FetchLeaderboardData()
    {
        ClearLeaderboard();

        var dbTask = dbReference.Child("leaderboard")
            .OrderByChild("completionTime")
            .LimitToFirst(10)
            .GetValueAsync();

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogWarning($"Failed to fetch leaderboard data: {dbTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = dbTask.Result;
        
        if (!snapshot.Exists)
        {
            Debug.Log("No leaderboard data found");
            yield break;
        }

        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            try
            {
                string json = childSnapshot.GetRawJsonValue();
                LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                leaderboardEntries.Add(entry);
                Debug.Log($"Loaded: {entry.userName} - {entry.completionTime}s");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing leaderboard entry: {ex.Message}");
            }
        }

        totalUsers = leaderboardEntries.Count;
        Debug.Log($"Total leaderboard entries: {totalUsers}");

        DisplayLeaderboard(leaderboardEntries);
    }

    /// <summary>
    /// Displays leaderboard entries in UI.
    /// </summary>
    /// <param name="entries"></param>
    private void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        if (leaderboardEntryPrefab == null || leaderboardContainer == null)
        {
            Debug.LogError("Leaderboard prefab or container not assigned!");
            return;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            LeaderboardEntry entry = entries[i];
            int rank = i + 1;

            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            entryObj.transform.localScale = Vector3.one;

            LeaderboardUI entryUI = entryObj.GetComponent<LeaderboardUI>();
            if (entryUI != null)
            {
                entryUI.SetEntry(rank, entry.userName, entry.completionTime);
            }
            else
            {
                Debug.LogWarning("LeaderboardUI component not found on prefab!");
            }
        }
    }

    /// <summary>
    /// Clears existing leaderboard entries from UI.
    /// </summary>
    private void ClearLeaderboard()
    {
        if (leaderboardContainer == null) return;

        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    #region Get Player Rank
    /// <summary>
    /// Gets the current player's rank in the leaderboard.
    /// </summary>
    public void GetMyRank()
    {
        StartCoroutine(GetPlayerRankCoroutine(auth.CurrentUser.UserId));
    }

    /// <summary>
    /// Coroutine to get player's rank from Firebase Realtime Database.
    /// </summary>
    private IEnumerator GetPlayerRankCoroutine(string userId)
    {
        // Get player's time
        var playerTask = dbReference.Child("leaderboard").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => playerTask.IsCompleted);

        if (playerTask.Exception != null || !playerTask.Result.Exists)
        {
            Debug.Log("Player has no leaderboard entry yet");
            yield break;
        }

        string json = playerTask.Result.GetRawJsonValue();
        LeaderboardEntry playerEntry = JsonUtility.FromJson<LeaderboardEntry>(json);
        float playerTime = playerEntry.completionTime;

        var rankTask = dbReference.Child("leaderboard")
            .OrderByChild("completionTime")
            .EndAt(playerTime)
            .GetValueAsync();

        yield return new WaitUntil(() => rankTask.IsCompleted);

        if (rankTask.Exception != null)
        {
            Debug.LogError($"Failed to get rank: {rankTask.Exception}");
            yield break;
        }

        int rank = (int)rankTask.Result.ChildrenCount;
        Debug.Log($"Your rank: {rank} with time: {playerTime}s");
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Gets the current timestamp in seconds.
    /// </summary>
    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
    /// <summary>
    /// Formats time in seconds to a string MM:SS.mmm
    /// </summary>
    public string FormatTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
    }
    #endregion
}

#region Data Classes
/// <summary>
/// Represents a leaderboard entry.
/// </summary>
[Serializable]
public class LeaderboardEntry
{
    public string userId;
    public string userName;
    public float completionTime;
    public long timestamp;
}
#endregion