/*
* Author: Kwek Sin En
* Date: 4/12/2025
* Description: Manages overall game state, item collection, timer, and UI updates
*/
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TextMeshProUGUI itemsCountText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI collectedtxt;

    [Header("Game State")]
    public int itemsCollected = 0;
    public int totalItems = 5;
    public List<string> collectedItemIds = new List<string>();

    [Header("References")]
    public Timer timerScript;
    public LeaderboardManager leaderboardManager;
    #region Unity Lifecycle
    /// <summary>
    /// Initializes singleton instance of GameManager.
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
    /// Initializes game state and UI on start.
    /// </summary>
    void Start()
    {
        UpdateItemsUI();
        UpdateTimerUI();
        LoadCollectedItems();
    }
    /// <summary>
    /// Updates timer UI every frame when timer is running.
    /// </summary>
    void Update()
    {
        if (timerScript != null && timerScript.IsTimerRunning())
        {
            UpdateTimerUI();
        }
    }
    #endregion
    #region UI Updates
    /// <summary>
    /// Updates timer UI.
    /// </summary>
    void UpdateTimerUI()
    {
        if (timerScript != null && timerText != null)
        {
            timerText.text = "Time: " + timerScript.GetFormattedTime();
        }
    }
    /// <summary>
    /// Updates items collected UI.
    /// </summary>
    private void UpdateItemsUI()
    {
        if (itemsCountText != null)
        {
            itemsCountText.text = "Items: " + itemsCollected.ToString() + "/" + totalItems.ToString();
        }
    }
    #endregion
    #region Load Data
    /// <summary>
    /// Loads collected items from Firebase.
    /// </summary>
    private async void LoadCollectedItems()
    {
        if (FirebaseManager.instance == null || !FirebaseManager.instance.IsUserLoggedIn())
            return;

        var user = FirebaseManager.instance.GetCurrentUser();
        if (user != null)
        {
            var playerData = await FirebaseManager.instance.GetPlayerData(user.UserId);
            if (playerData != null && playerData.itemsCollected != null)
            {
                itemsCollected = playerData.itemsCollected.Count;
                collectedItemIds = playerData.itemsCollected.Select(i => i.itemId).ToList();
                UpdateItemsUI();
            }
        }
    }
    #endregion
    #region Item Collection
    /// <summary>
    /// Marks an item as collected, updates UI, and saves to Firebase.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="itemName"></param>
    public async void CollectItem(string itemId, string itemName)
    {
        collectedtxt.text = "";
        if (collectedItemIds.Contains(itemId))
        {
            collectedtxt.text = "Item already collected!";
            await Task.Delay(2000);
            collectedtxt.text = "";
            Debug.Log($"Item {itemId} already collected!");
            return;
        }
        // Start timer on first item
        if (itemsCollected == 0 && timerScript != null && !timerScript.IsTimerRunning())
        {
            timerScript.StartTimer();
            Debug.Log("First item collected, timer started!");
        }
        itemsCollected++;
        collectedItemIds.Add(itemId);
        UpdateItemsUI();
        Debug.Log($"Items Collected: {itemsCollected}");

        await SaveItemToFirebase(itemId, itemName);

        // Check if all items collected
        if (itemsCollected >= totalItems)
        {
            await CompleteSet();
        }
    }
    /// <summary>
    /// Saves collected item to Firebase under the user's data.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="itemName"></param>
    private async Task SaveItemToFirebase(string itemId, string itemName)
    {
        if (FirebaseManager.instance == null || !FirebaseManager.instance.IsUserLoggedIn())
        {
            Debug.LogWarning("Cannot save item: User not logged in");
            return;
        }

        var user = FirebaseManager.instance.GetCurrentUser();
        if (user != null)
        {
            var playerData = await FirebaseManager.instance.GetPlayerData(user.UserId);
            if (playerData != null)
            {
                var newItem = new Inventory(itemId, itemName);
                playerData.itemsCollected.Add(newItem);
                await FirebaseManager.instance.UpdateUserData(user.UserId, playerData);
                Debug.Log($"Item {itemName} saved to Firebase!");
            }
        }
    }
    #endregion
    #region Set Completion
    /// <summary>
    /// Handles actions upon completing the full set of items.
    /// </summary>
    private async Task CompleteSet()
    {
        Debug.Log("Set Complete! All 5 items collected!");
        
        if (timerScript != null)
        {
            // Stop the timer and get final time
            timerScript.StopTimer();
            float finalTime = timerScript.GetCurrentTime();
            Debug.Log($"Final completion time: {timerScript.GetFormattedTime()}");
            
            // Save to Firebase
            await SaveSetCompletion(finalTime);
            
            // Submit to leaderboard
            if (leaderboardManager != null)
            {
                leaderboardManager.SubmitScore(finalTime);
                // Show leaderboard after a delay
                await Task.Delay(2000);
                leaderboardManager.ShowLeaderboard();
            }
        }
    }
    /// <summary>
    /// Saves set completion data to Firebase.
    /// </summary>
    /// <param name="completionTime"></param>
    private async Task SaveSetCompletion(float completionTime)
    {
        if (FirebaseManager.instance == null || !FirebaseManager.instance.IsUserLoggedIn())
            return;

        var user = FirebaseManager.instance.GetCurrentUser();
        if (user != null)
        {
            var playerData = await FirebaseManager.instance.GetPlayerData(user.UserId);
            if (playerData != null)
            {
                playerData.completedSet = true;
                playerData.completedAt = System.DateTime.UtcNow.ToString("o");
                playerData.timeTaken = completionTime;
                
                await FirebaseManager.instance.UpdateUserData(user.UserId, playerData);
                Debug.Log("Set completion saved to Firebase with time!");
            }
        }
    }
    #endregion
    #region Scene Management
    /// <summary>
    /// Shows or hides UI elements based on the loaded scene.
    /// </summary>
    public void ShowUIOnSceneLoad(int sceneIndex)
    {
        if (sceneIndex == 1)
        {
            itemsCountText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);
        }
        else
        {
            itemsCountText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
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