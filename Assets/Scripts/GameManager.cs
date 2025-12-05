using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI itemsCountText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI collectedtxt;
    public int itemsCollected = 0;
    public int totalItems = 5;
    public Timer timerScript;
    private List<string> collectedItemIds = new List<string>();
    private void Awake()
    {
        // Ensure that there is only one instance of GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }
    void Start()
    {
        UpdateItemsUI();
        UpdateTimerUI();
        LoadCollectedItems();
    }
    void Update()
    {
        if (timerScript != null && timerScript.IsTimerRunning())
        {
            UpdateTimerUI();
        }
    }
    void UpdateTimerUI()
    {
        if (timerScript != null && timerText != null)
        {
            timerText.text = "Time: " + timerScript.GetFormattedTime();
        }
    }
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
    public async void CollectItem(string itemId, string itemName)
    {
        collectedtxt.text = "";
        // Check if already collected
        if (collectedItemIds.Contains(itemId))
        {
            collectedtxt.text = "Item already collected!";
            await Task.Delay(2000);
            collectedtxt.text = "";
            Debug.Log($"Item {itemId} already collected!");
            return;
        }
        // Start timer on first item collection
        if (itemsCollected == 0 && timerScript != null && !timerScript.IsTimerRunning())
        {
            timerScript.StartTimer();
            Debug.Log("First item collected, timer started!");
        }
        itemsCollected++;
        collectedItemIds.Add(itemId);
        UpdateItemsUI();
        Debug.Log($"Items Collected: {itemsCollected}");

        // Save to Firebase
        await SaveItemToFirebase(itemId, itemName);

        // Check if set is complete
        if (itemsCollected >= totalItems)
        {
            await CompleteSet();
        }
    }
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
            // Get current player data
            var playerData = await FirebaseManager.instance.GetPlayerData(user.UserId);
            if (playerData != null)
            {
                // Add new item
                var newItem = new Inventory(itemId, itemName);
                playerData.itemsCollected.Add(newItem);

                // Update Firebase
                await FirebaseManager.instance.UpdateUserData(user.UserId, playerData);
                Debug.Log($"Item {itemName} saved to Firebase!");
            }
        }
    }
    private async Task CompleteSet()
    {
        Debug.Log("Set Complete! All 5 items collected!");
        
        if (timerScript != null)
        {
            timerScript.StopTimer();
            float finalTime = timerScript.GetCurrentTime();
            
            Debug.Log($"Final completion time: {timerScript.GetFormattedTime()}");
            
            // Save to player data and leaderboard
            await SaveSetCompletion(finalTime);
        }
    }
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
                playerData.timeTaken = completionTime; // Save the time taken
                
                await FirebaseManager.instance.UpdateUserData(user.UserId, playerData);
                Debug.Log("Set completion saved to Firebase with time!");
                
                OnSetCompleted(completionTime);
            }
        }
    }
    private void OnSetCompleted(float completionTime)
    {
        // Implement actions to take when the set is completed
    }
    private void UpdateItemsUI()
    {
        if (itemsCountText != null)
        {
            itemsCountText.text = "Items: " + itemsCollected.ToString() + "/" + totalItems.ToString();
        }
    }
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
    public void Quit()
    {
        Debug.Log("Quit");
        // Quit the application
        Application.Quit();
    }
}
