using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject; // Assign the root of the jar in Inspector

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
        GameManager.instance.ModifyItemsCount();
    }
}


