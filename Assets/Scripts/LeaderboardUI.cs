using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LeaderboardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text rankText;
    public TMP_Text playerNameText;
    public TMP_Text timeText;
    public Image backgroundImage;

    [Header("Rank Colors")]
    public Color firstPlaceColor = new Color(1f, 0.84f, 0f); // Gold
    public Color secondPlaceColor = new Color(0.75f, 0.75f, 0.75f); // Silver
    public Color thirdPlaceColor = new Color(0.8f, 0.5f, 0.2f); // Bronze
    public Color defaultColor = Color.white;
    public void SetEntry(int rank, string playerName, float completionTime)
    {
        // Set rank
        if (rankText != null)
            rankText.text = $"#{rank}";

        // Set player name
        if (playerNameText != null)
            playerNameText.text = playerName;

        // Set time with formatting
        if (timeText != null)
            timeText.text = FormatTime(completionTime);

        if (backgroundImage != null)
        {
            backgroundImage.color = rank switch
            {
                1 => firstPlaceColor,
                2 => secondPlaceColor,
                3 => thirdPlaceColor,
                _ => defaultColor
            };
        }

        if (rankText != null && rank <= 3)
        {
            rankText.fontStyle = FontStyles.Bold;
            rankText.fontSize += 2;
        }
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        int milliseconds = Mathf.FloorToInt((seconds * 1000f) % 1000f);
        
        return $"{minutes:00}:{secs:00}.{milliseconds:000}";
    }
}
