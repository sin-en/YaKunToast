using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject leaderboard;

    [Header("References")]
    public LeaderboardManager leaderboardManager;

    private void Start()
    {
        ShowMainMenu();
    }

    #region Panel Navigation
    public void ShowMainMenu()
    {
        if (mainMenu != null)
            mainMenu.SetActive(true);
        
        if (leaderboard != null)
            leaderboard.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        
        if (leaderboard != null)
            leaderboard.SetActive(true);

        // Fetch and display leaderboard
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
    }
    #endregion

    #region Button Callbacks
    public void OnLeaderboardButton()
    {
        ShowLeaderboard();
    }

    public void OnBackButton()
    {
        ShowMainMenu();
    }

    public void OnRefreshLeaderboard()
    {
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
    }
    #endregion
}