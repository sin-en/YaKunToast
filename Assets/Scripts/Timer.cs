/*
* Author: Kwek Sin En
* Date: 26/11/2025
* Description: Manages the in-game timer functionality
*/
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timer = 0f;
    public bool isRunning = false;

    [Header("UI References")]
    public TMP_Text timerText;

    [Header("References")]
    public LeaderboardManager leaderboardManager;

    private FirebaseAuth auth;
    private DatabaseReference dbReference;

    /// <summary>
    /// Initializes Firebase references
    /// </summary>
    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Updates the timer every frame when running
    /// </summary>
    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    #region Timer Control
    /// <summary>
    /// Starts the timer from 0
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        timer = 0f;
        Debug.Log("Timer started!");
    }

    /// <summary>
    /// Pauses the timer
    /// </summary>
    public void PauseTimer()
    {
        isRunning = false;
        Debug.Log($"Timer paused at: {FormatTime(timer)}");
    }

    /// <summary>
    /// Resumes the timer
    /// </summary>
    public void ResumeTimer()
    {
        isRunning = true;
        Debug.Log("Timer resumed!");
    }

    /// <summary>
    /// Stops the timer and resets to 0
    /// </summary>
    public void ResetTimer()
    {
        isRunning = false;
        timer = 0f;
        UpdateTimerDisplay();
        Debug.Log("Timer reset!");
    }

    /// <summary>
    /// Stops the timer when all items are collected
    /// </summary>
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
        StartCoroutine(SaveCompletionTimeCoroutine(finalTime));
    }
    #endregion

    #region Save to Firebase
    /// <summary>
    /// Saves the completion time to Firebase and submits to leaderboard
    /// </summary>
    /// <param name="completionTime"></param>
    /// <returns></returns>
    private IEnumerator SaveCompletionTimeCoroutine(float completionTime)
    {
        // Submit to leaderboard manager (this handles all Firebase saving)
        if (leaderboardManager != null)
        {
            leaderboardManager.SubmitScore(completionTime);
            Debug.Log($"Completion time submitted to leaderboard: {FormatTime(completionTime)}");
        }
        else
        {
            Debug.LogError("LeaderboardManager reference not set!");
        }
        
        yield return null;
    }
    #endregion

    #region UI Display
    /// <summary>
    /// Updates the timer display UI
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(timer);
        }
    }

    /// <summary>
    /// Formats time in seconds to a string MM:SS.mm
    /// </summary>
    /// <param name="timeInSeconds"></param>
    /// <returns></returns>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }

    /// <summary>
    /// Gets the current timer value as a formatted string
    /// </summary>
    public string GetFormattedTime()
    {
        return FormatTime(timer);
    }

    /// <summary>
    /// Gets the current timer value in seconds
    /// </summary>
    public float GetCurrentTime()
    {
        return timer;
    }
    #endregion

    #region Public Accessors
    /// <summary>
    /// Check if the timer is currently running
    /// </summary>
    public bool IsTimerRunning()
    {
        return isRunning;
    }
    #endregion
}