/*
* Author: Kwek Sin En
* Date: 16/11/2025
* Description: Manages multiple image tracking in the Unity AR game
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImageTrackingManager : MonoBehaviour
{
    [SerializeField] 
    private List<GameObject> prefabsToSpawn = new List<GameObject>();

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> arObjects;

    private void Start()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (trackedImageManager == null)
        {
            Debug.LogError("No ARTrackedImageManager found on this GameObject!");
            return;
        }

        arObjects = new Dictionary<string, GameObject>();

        SetupPrefabs();
        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDestroy()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    /// <summary>
    /// Instantiates each prefab once and stores it in a dictionary.
    /// </summary>
    private void SetupPrefabs()
    {
        foreach (GameObject prefab in prefabsToSpawn)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Null prefab found in list!");
                continue;
            }

            GameObject spawned = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            spawned.name = prefab.name;
            spawned.SetActive(false);

            if (!arObjects.ContainsKey(spawned.name))
                arObjects.Add(spawned.name, spawned);
            else
                Debug.LogWarning($"Duplicate prefab name detected: {spawned.name}");
        }
    }

    /// <summary>
    /// Handles image tracking changes
    /// </summary>
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
            UpdateARObject(trackedImage);

        foreach (var trackedImage in eventArgs.updated)
            UpdateARObject(trackedImage);

        foreach (var trackedImage in eventArgs.removed)
            HideARObject(trackedImage);
    }

    private void HideARObject(KeyValuePair<TrackableId, ARTrackedImage> trackedImage)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates prefab visibility and position when image is tracked.
    /// </summary>
    private void UpdateARObject(ARTrackedImage trackedImage)
    {
        if (trackedImage == null || trackedImage.referenceImage == null)
            return;

        string imageName = trackedImage.referenceImage.name;

        if (!arObjects.ContainsKey(imageName))
        {
            Debug.LogWarning($"No prefab found for image: {imageName}");
            return;
        }

        GameObject obj = arObjects[imageName];

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            obj.SetActive(true);
            obj.transform.position = trackedImage.transform.position;
            obj.transform.rotation = trackedImage.transform.rotation;
        }
        else
        {
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// Called when image is removed from tracking.
    /// </summary>
    private void HideARObject(ARTrackedImage trackedImage)
    {
        if (trackedImage == null) return;

        string imageName = trackedImage.referenceImage.name;

        if (arObjects.ContainsKey(imageName))
            arObjects[imageName].SetActive(false);
    }
}