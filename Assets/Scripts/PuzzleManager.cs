using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Models")]
    public GameObject[] modelsToSpawn; // 5 models to spawn
    public GameObject finalModel; // The model that appears after all 5 are collected
    
    [Header("Spawn Settings")]
    public Transform spawnParent; // Will be set by spawner script
    public Vector3[] spawnOffsets; // Positions relative to tracked image
    
    [Header("Box")]
    public GameObject dropBox; // The box where models are dragged
    
    private List<GameObject> spawnedModels = new List<GameObject>();
    private int modelsDropped = 0;
    private int totalModels = 5;

    void Start()
    {
        if (finalModel != null)
            finalModel.SetActive(false);
    }

    public void SpawnModels(Transform parent)
    {
        // Only spawn once
        if (spawnedModels.Count > 0) return;
        
        spawnParent = parent;
        
        DebugDisplay.Log("=== SpawnModels called! ===");
        Debug.Log("=== SpawnModels called! ===");
        
        for (int i = 0; i < totalModels && i < modelsToSpawn.Length; i++)
        {
            Vector3 spawnPos = parent.position;
            if (i < spawnOffsets.Length)
                spawnPos += parent.TransformDirection(spawnOffsets[i]);
            
            GameObject spawned = Instantiate(modelsToSpawn[i], spawnPos, parent.rotation, parent);
            
            DebugDisplay.Log($"Spawned: {spawned.name}");
            Debug.Log($"Spawned: {spawned.name}");
            
            // Check what's actually on this object
            Component[] components = spawned.GetComponents<Component>();
            DebugDisplay.Log($"  Components: {components.Length}");
            foreach (var comp in components)
            {
                DebugDisplay.Log($"    - {comp.GetType().Name}");
            }
            
            // Check children colliders
            Collider[] collidersInChildren = spawned.GetComponentsInChildren<Collider>();
            DebugDisplay.Log($"  Colliders in children: {collidersInChildren.Length}");
            
            // Add required components
            if (spawned.GetComponent<Collider>() == null)
            {
                MeshCollider meshCol = spawned.AddComponent<MeshCollider>();
                meshCol.convex = true;
                meshCol.isTrigger = false;
                DebugDisplay.Log($"Added MeshCollider to {spawned.name}");
                Debug.Log($"Added MeshCollider to {spawned.name}");
            }
            else
            {
                // If it already has a collider, configure it properly
                Collider col = spawned.GetComponent<Collider>();
                col.isTrigger = false;
                
                MeshCollider existingMesh = col as MeshCollider;
                if (existingMesh != null)
                {
                    existingMesh.convex = true;
                    DebugDisplay.Log($"Set {spawned.name} collider: convex=true, trigger=false");
                    Debug.Log($"Set {spawned.name} collider: convex=true, trigger=false");
                }
            }
            
            if (spawned.GetComponent<DraggableARObject>() == null)
            {
                spawned.AddComponent<DraggableARObject>();
                DebugDisplay.Log($"Added DraggableARObject to {spawned.name}");
                Debug.Log($"Added DraggableARObject to {spawned.name}");
            }
            
            spawnedModels.Add(spawned);
            
            // Final check
            Collider finalCol = spawned.GetComponent<Collider>();
            DebugDisplay.Log($"FINAL CHECK {spawned.name}:");
            DebugDisplay.Log($"  Has collider: {finalCol != null}");
            if (finalCol != null)
            {
                DebugDisplay.Log($"  Trigger: {finalCol.isTrigger}");
                DebugDisplay.Log($"  Enabled: {finalCol.enabled}");
            }
        }
        
        DebugDisplay.Log($"Total spawned: {spawnedModels.Count}");
        Debug.Log($"Total spawned: {spawnedModels.Count}");
    }

    public void ObjectDropped(GameObject droppedObject)
    {
        modelsDropped++;
        DebugDisplay.Log($"Object dropped! Count: {modelsDropped}/{totalModels}");
        Debug.Log($"Object dropped! Count: {modelsDropped}/{totalModels}");
        
        Destroy(droppedObject);
        
        if (modelsDropped >= totalModels)
        {
            ShowFinalModel();
        }
    }

    void ShowFinalModel()
    {
        DebugDisplay.Log("Showing final model!");
        Debug.Log("Showing final model!");
        
        if (finalModel != null)
        {
            finalModel.SetActive(true);
            if (spawnParent != null)
                finalModel.transform.position = spawnParent.position;
        }
    }
}