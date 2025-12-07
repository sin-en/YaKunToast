using UnityEngine;

public class DragObject : MonoBehaviour
{
    private float zCoord;

    void OnMouseDown()
    {
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
    }

    void OnMouseDrag()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = zCoord;
        transform.position = Camera.main.ScreenToWorldPoint(pos);
    }
}
