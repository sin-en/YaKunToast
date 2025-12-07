using UnityEngine;

public class SnapZone : MonoBehaviour
{
    public int itemsToPlace = 5;       // how many puzzle pieces required
    private int itemsPlaced = 0;       // counter
    public GameObject finalModel;      // drag your final model here in inspector

    private void Start()
    {
        finalModel.SetActive(false);   // final model hidden at start
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PuzzleItem")) // make sure your 5 models have this tag
        {
            itemsPlaced++;

            // Remove the dropped item visually
            Destroy(other.gameObject);          // ← makes it disappear permanently
            // alternative: other.gameObject.SetActive(false); (if you want to hide instead of destroy)

            // Check if completed
            if (itemsPlaced >= itemsToPlace)
            {
                finalModel.SetActive(true);
                Debug.Log("All pieces placed — final model revealed!");
            }
        }
    }
}
