using UnityEngine;

public class DropBox : MonoBehaviour
{
    [HideInInspector] public FinalPuzzleManager manager;
    public int collectedCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PuzzlePiece"))
        {
            collectedCount++;
            other.gameObject.SetActive(false);

            if(manager != null)
                manager.CheckCompletion();
        }
    }
}
