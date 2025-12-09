/*
* Author: Kwek Sin En
* Date: 11/11/2025
* Description: Handles button interactions for collectible items in the Unity game
*/
using UnityEngine;
using System.Collections;

public class ButtonBehaviour : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField]
    private GameObject targetObject; 
    [SerializeField]
    private GameObject historyUI;
    public bool isCollected;
    public string itemId;
    public string itemName;
    /// <summary>
    /// Initializes the itemId based on the GameObject's name if not set.
    /// </summary>
    void Awake()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = gameObject.name;
        }
    }
    /// <summary>
    /// Checks if the item has been collected at the start of the game.
    /// </summary>
    void Start()
    {
        CheckIfCollected();
    }
    /// <summary>
    /// Checks if the item has been collected by querying the GameManager.
    /// </summary>
    private void CheckIfCollected()
    {
        if (GameManager.instance != null)
        {
            isCollected = GameManager.instance.collectedItemIds.Contains(itemId);
        }
    }
    /// <summary>
    /// Toggles the MeshRenderer components of the target object and its children.
    /// </summary>
    public void ToggleMeshRenderer()
    {
        MeshRenderer[] renderers = targetObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            r.enabled = !r.enabled;
        }
    }
    /// <summary>
    /// Handles button click events to show history UI or collected message.
    /// </summary>
    public void OnButtonClick()
    {
        CheckIfCollected();
        if (isCollected)
        {
            if (GameManager.instance != null && GameManager.instance.collectedtxt != null)
            {
                StartCoroutine(ShowCollectedMessage());
            }
        }
        else
        {
            if (historyUI != null)
            {
                historyUI.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Displays a message indicating the item has already been collected.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowCollectedMessage()
    {
        GameManager.instance.collectedtxt.text = "Item already collected!";
        yield return new WaitForSeconds(2f);
        GameManager.instance.collectedtxt.text = "";
    }
    /// <summary>
    /// Marks the item as collected and updates the GameManager and UI.
    /// </summary>
    public void CollectItem()
    {
        isCollected = true;
        if (GameManager.instance != null)
        {
            GameManager.instance.CollectItem(itemId, itemName);
        }
        if (historyUI != null)
        {
            historyUI.SetActive(false);
        }
    }
}


