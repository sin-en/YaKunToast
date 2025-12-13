using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastTest : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            
            DebugDisplay.Log($"RaycastTest - Click at: {mousePos}");
            
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                DebugDisplay.Log($"RaycastTest HIT: {hit.collider.gameObject.name}");
                Debug.Log($"RaycastTest HIT: {hit.collider.gameObject.name}");
            }
            else
            {
                DebugDisplay.Log("RaycastTest: Hit nothing");
                
                // Check all colliders in scene
                Collider[] allColliders = FindObjectsOfType<Collider>();
                DebugDisplay.Log($"Total colliders in scene: {allColliders.Length}");
                
                // List first few
                for (int i = 0; i < Mathf.Min(5, allColliders.Length); i++)
                {
                    DebugDisplay.Log($"  - {allColliders[i].gameObject.name}");
                }
            }
        }
    }
}