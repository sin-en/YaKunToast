using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultipleImageTrackingManager : MonoBehaviour
{
    //Prefabs to spawn
    [SerializeField]
    List<GameObject> prefabsToSpawn = new List<GameObject>();
    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> arObjects;
    private void Start()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (trackedImageManager == null) return;
        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        arObjects = new Dictionary<string, GameObject>();
        SetupSceneElements();
    }
    private void OnDestroy()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }
    //Setup Scene Elements
    private void SetupSceneElements()
    {
        foreach (var prefab in prefabsToSpawn)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Null prefab found in prefabsToSpawn list!");
                continue;
            }
            var arObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            arObject.name = prefab.name;
            arObject.gameObject.SetActive(false);
            arObjects.Add(arObject.name, arObject);
        }
    }
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateARObject(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateARObject(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            UpdateARObject(trackedImage.Value);
        }
    }
    private void UpdateARObject(ARTrackedImage trackedImage)
    {
        if(trackedImage == null) return;
        if (trackedImage.trackingState is UnityEngine.XR.ARSubsystems.TrackingState.Limited or UnityEngine.XR.ARSubsystems.TrackingState.None)
        {
            arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            return;
        }
        arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
        arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
        arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
    }
}
