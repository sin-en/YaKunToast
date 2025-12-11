using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public ImageTrackerSpawner manager;
    public float acceptedDistance = 0.3f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Drag();
        CheckPlacement();
    }

    void Drag()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 pos = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0.5f));
                transform.position = pos;
            }
        }
    }

    void CheckPlacement()
    {
        Vector3 target = cam.transform.position + cam.transform.forward * 0.5f; // front of player

        float distance = Vector3.Distance(transform.position, target);

        if (distance < acceptedDistance)
        {
            manager.ObjectPlacedCorrectly(gameObject);
        }
    }
}
