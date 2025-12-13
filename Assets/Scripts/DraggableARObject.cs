using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;

public class DraggableARObject : MonoBehaviour
{
    private Camera arCamera;
    private PuzzleManager puzzleManager;
    private bool isDragging = false;
    private Vector3 offset;
    private float dragDistance = 10f; // Distance from camera to drag plane

    void Start()
    {
        arCamera = Camera.main;
        puzzleManager = FindObjectOfType<PuzzleManager>();
        
        // Ensure this object is on Default layer for raycasting
        gameObject.layer = LayerMask.NameToLayer("Default");
        
        Debug.Log($"=== DraggableARObject Start on {gameObject.name} ===");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Layer: {LayerMask.LayerToName(gameObject.layer)}");
        Debug.Log($"AR Camera found: {arCamera != null}");
        Debug.Log($"PuzzleManager found: {puzzleManager != null}");
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Debug.Log($"Collider type: {col.GetType().Name}");
            Debug.Log($"Collider bounds: {col.bounds.size}");
            Debug.Log($"Collider enabled: {col.enabled}");
            Debug.Log($"Collider isTrigger: {col.isTrigger}");
        }
        else
        {
            Debug.LogError($"NO COLLIDER on {gameObject.name}!");
        }
    }

    void Update()
    {
        HandleMouseInput();
        HandleTouchInput();
    }

    void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            TryStartDrag(mousePosition);
        }
        else if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            ContinueDrag(mousePosition);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            EndDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;
        if (!touch.press.isPressed) 
        {
            if (isDragging) EndDrag();
            return;
        }

        Vector2 touchPosition = touch.position.ReadValue();
        var phase = touch.phase.ReadValue();

        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            TryStartDrag(touchPosition);
        }
        else if (isDragging && (phase == UnityEngine.InputSystem.TouchPhase.Moved || phase == UnityEngine.InputSystem.TouchPhase.Stationary))
        {
            ContinueDrag(touchPosition);
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            if (isDragging) EndDrag();
        }
    }

    void TryStartDrag(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        Debug.Log($"=== Checking click at {screenPosition} ===");
        Debug.Log($"Ray origin: {ray.origin}, direction: {ray.direction}");

        // Use RaycastAll to see everything we're hitting
        RaycastHit[] allHits = Physics.RaycastAll(ray, 100f);
        Debug.Log($"RaycastAll found {allHits.Length} objects:");
        foreach (var h in allHits)
        {
            Debug.Log($"  - {h.collider.gameObject.name} at distance {h.distance}");
        }

        // Now do specific raycast for this object
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log($"Primary hit: {hit.collider.gameObject.name}");
            
            // Check if we hit this object or any of its children
            Transform hitTransform = hit.collider.transform;
            bool isThisObject = hitTransform == transform || hitTransform.IsChildOf(transform);
            
            Debug.Log($"Hit transform: {hitTransform.name}, This transform: {transform.name}, IsChild: {hitTransform.IsChildOf(transform)}");
            
            if (isThisObject)
            {
                Debug.Log($"✓ Started dragging {gameObject.name}!");
                isDragging = true;
                
                // Calculate offset based on hit point
                dragDistance = Vector3.Distance(arCamera.transform.position, hit.point);
                offset = transform.position - hit.point;
            }
            else
            {
                Debug.Log($"❌ Hit {hitTransform.name} but we're looking for {transform.name}");
            }
        }
        else
        {
            Debug.Log("Raycast hit nothing");
        }
    }

    void ContinueDrag(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        
        // Project the ray to the drag distance
        Vector3 targetPosition = ray.origin + ray.direction * dragDistance;
        transform.position = targetPosition + offset;
    }

    void EndDrag()
    {
        Debug.Log($"Ended dragging {gameObject.name}");
        isDragging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropBox") && puzzleManager != null)
        {
            Debug.Log($"{gameObject.name} entered DropBox!");
            puzzleManager.ObjectDropped(gameObject);
        }
    }

    // Visual debugging
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = isDragging ? Color.green : Color.yellow;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}