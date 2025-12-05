using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject; // Assign the root of item in Inspector
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

    public void ToggleMeshRenderer()
    {
        MeshRenderer[] renderers = targetObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            r.enabled = !r.enabled;
        }
    }

    public void CollectItem()
    {
        isCollected = true;
        if (GameManager.instance != null)
            {
                GameManager.instance.CollectItem(itemId, itemName);
            }
    }
}


