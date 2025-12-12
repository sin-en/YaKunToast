/*
* Author: Kwek Sin En
* Date: 9/12/2025
* Description: Manages game menu navigation and panel visibility
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager instance;

    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject leaderboard;
    public GameObject ArGame;

    [Header("References")]
    public LeaderboardManager leaderboardManager;

    /// <summary>
    /// Initializes singleton instance of GameMenuManager.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes menu state on start.
    /// </summary>
    private void Start()
    {
        ShowMainMenu();
    }

    #region Panel Navigation
    /// <summary>
    /// Shows the main menu panel.
    /// </summary>
    public void ShowMainMenu()
    {
        if (mainMenu != null)
            mainMenu.SetActive(true);
        
        if (leaderboard != null)
            leaderboard.SetActive(false);
        if (ArGame != null)
        {
            ArGame.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the leaderboard panel.
    /// </summary>
    public void ShowLeaderboard()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        
        if (leaderboard != null)
            leaderboard.SetActive(true);

        if (ArGame != null)
            ArGame.SetActive(false);

        // Fetch and display leaderboard
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
    }

    /// <summary>
    /// Shows the AR game.
    /// </summary>
    public void ShowARGame()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        
        if (leaderboard != null)
            leaderboard.SetActive(false);

        if (ArGame != null)
            ArGame.SetActive(true);
    }
    #endregion

    #region Button Callbacks
    /// <summary>
    /// Handles Play button click.
    /// </summary>
    public void OnPlayButton()
    {
        ShowARGame();
    }

    /// <summary>
    /// Handles Leaderboard button click.
    /// </summary>
    public void OnLeaderboardButton()
    {
        ShowLeaderboard();
    }

    /// <summary>
    /// Handles Back button click.
    /// </summary>
    public void OnBackButton()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Handles Refresh Leaderboard button click.
    /// </summary>
    public void OnRefreshLeaderboard()
    {
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
    }
    #endregion
    
    #region Utility
    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    #endregion
}