using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Collider))]
public class ARDragObject : MonoBehaviour
{
    private Camera arCamera;
    private ARRaycastManager raycastManager;
    private bool isDragging = false;
    private Vector2 touchPosition;

    void Start()
    {
        arCamera = Camera.main;
        raycastManager = FindObjectOfType<ARRaycastManager>();
        if (raycastManager == null)
            Debug.LogError("ARRaycastManager not found in scene!");
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            isDragging = false;
            return;
        }

        Touch touch = Input.GetTouch(0);
        touchPosition = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                    isDragging = true;
            }
        }

        if (isDragging && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
            {
                transform.position = hits[0].pose.position;
            }
            else
            {
                // fallback: move in front of camera at 0.5m
                transform.position = arCamera.transform.position + arCamera.transform.forward * 0.5f;
            }
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }
}
