/*
* Description: Spawns the 5 draggable models when this prefab is activated by image tracking
*/

using UnityEngine;

public class DraggableModelsSpawner : MonoBehaviour
{
    private PuzzleManager puzzleManager;
    private bool hasSpawned = false;

    void OnEnable()
    {
        Debug.Log("=== DraggableModelsSpawner OnEnable called! ===");
        DebugDisplay.Log("=== DraggableModelsSpawner OnEnable ===");
        
        // Find PuzzleManager when enabled (in case it wasn't found yet)
        if (puzzleManager == null)
        {
            puzzleManager = FindObjectOfType<PuzzleManager>();
            Debug.Log($"Searching for PuzzleManager... Found: {puzzleManager != null}");
            DebugDisplay.Log($"PuzzleManager found: {puzzleManager != null}");
        }

        if (puzzleManager == null)
        {
            Debug.LogError("PuzzleManager not found in scene!");
            DebugDisplay.Log("ERROR: PuzzleManager not found!");
            return;
        }

        // Spawn models when this prefab becomes active (when image is tracked)
        if (!hasSpawned)
        {
            Debug.Log("Calling SpawnModels...");
            DebugDisplay.Log("Calling SpawnModels...");
            puzzleManager.SpawnModels(transform);
            hasSpawned = true;
            Debug.Log("Models spawned!");
            DebugDisplay.Log("SpawnModels complete!");
        }
        else
        {
            Debug.Log("Already spawned, skipping.");
            DebugDisplay.Log("Already spawned");
        }
    }
}