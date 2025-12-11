using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackerSpawner : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject[] objectsToSpawn;   // The 5 draggable objects
    public GameObject finalObject;        // Object that appears at the end
    public Transform spawnParent;         // Use ARSessionOrigin or empty object

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private int placedCount = 0;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.added)
        {
            SpawnObjects(image);
        }
    }

    void SpawnObjects(ARTrackedImage img)
    {
        foreach (var obj in objectsToSpawn)
        {
            GameObject newObj = Instantiate(obj, img.transform.position, Quaternion.identity, spawnParent);
            spawnedObjects.Add(newObj);
        }
    }

    public void ObjectPlacedCorrectly(GameObject obj)
    {
        obj.SetActive(false);
        placedCount++;

        if (placedCount == objectsToSpawn.Length)
        {
            // All 5 objects done → spawn final object
            finalObject.SetActive(true);
        }
    }
}
