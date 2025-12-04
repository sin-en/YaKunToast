using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The singleton instance of the GameManager.
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// UI Text element to display the item counter.
    /// </summary>
    public TextMeshProUGUI itemsCountText;
    public int itemsCollected = 0;
    public int totalItems = 5;

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
        itemsCountText.text = "Items: " + itemsCollected.ToString() + "/" + totalItems.ToString();
    }

    /// <summary>
    /// Updates the item counter UI text.
    /// </summary>
    public void ModifyItemsCount()
    {
        itemsCollected++;
        UpdateItemsUI();
        Debug.Log("Items Collected: " + itemsCollected);
    }

    /// <summary>
    /// Updates the UI text to reflect the current item count.
    /// </summary>
    private void UpdateItemsUI()
    {
        itemsCountText.text = $"Items Collected: {itemsCollected}/{totalItems}";
    }

    /// <summary>
    /// Show items UI when scene loaded is 1.
    /// </summary>
    public void ShowItemsUIOnSceneLoad(int sceneIndex)
    {
        if (sceneIndex == 1)
        {
            itemsCountText.gameObject.SetActive(true);
        }
        else
        {
            itemsCountText.gameObject.SetActive(false);
        }
    }
}
