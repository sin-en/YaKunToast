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
    
    void Awake()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = gameObject.name;
        }
    }
    
    void Start()
    {
        CheckIfCollected();
    }
    
    private void CheckIfCollected()
    {
        if (GameManager.instance != null)
        {
            isCollected = GameManager.instance.collectedItemIds.Contains(itemId);
        }
    }
    
    public void ToggleMeshRenderer()
    {
        MeshRenderer[] renderers = targetObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            r.enabled = !r.enabled;
        }
    }
    
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
    
    private IEnumerator ShowCollectedMessage()
    {
        GameManager.instance.collectedtxt.text = "Item already collected!";
        yield return new WaitForSeconds(2f);
        GameManager.instance.collectedtxt.text = "";
    }
    
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


