/*
* Author: Kwek Sin En
* Date: 26/11/2025
* Description: Manages the UI elements for displaying leaderboard entries in the Unity game
*/
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LeaderboardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text rankText;
    public TMP_Text playerNameText;
    public TMP_Text timeText;
    [Header("Rank Colors")]
    public Color firstPlaceColor = new Color(1f, 0.84f, 0f); // Gold
    public Color secondPlaceColor = new Color(0.75f, 0.75f, 0.75f); // Silver
    public Color thirdPlaceColor = new Color(0.8f, 0.5f, 0.2f); // Bronze
    public Color defaultColor = Color.white;

    /// <summary>
    /// Sets the leaderboard entry UI based on rank, player name, and completion time.
    /// </summary>
    /// <param name="rank"></param>
    /// <param name="playerName"></param>
    /// <param name="completionTime"></param>
    public void SetEntry(int rank, string playerName, float completionTime)
    {
        Color rankColor = rank switch
        {
            1 => firstPlaceColor,
            2 => secondPlaceColor,
            3 => thirdPlaceColor,
            _ => defaultColor
        };
        // Set rank text
        if (rankText != null)
        {
            rankText.text = $"#{rank}";
            rankText.color = rankColor;
            // Make top 3 bigger
            if (rank <= 3)
            {
                rankText.fontSize += 2;
            }
        }
        // Set player name text
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
            playerNameText.color = rankColor;
            if (rank <= 3)
            {
                playerNameText.fontSize += 2;
            }
        }
        // Set time text
        if (timeText != null)
        {
            timeText.text = FormatTime(completionTime);
            timeText.color = rankColor;
            if (rank <= 3)
            {
                timeText.fontSize += 2;
            }
        }
    }

    /// <summary>
    /// Formats time in seconds to a string MM:SS.mmm
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }
}
