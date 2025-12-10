/*
* Author: Kwek Sin En
* Date: 3/12/2025
* Description: Handles stamp collection and UI display
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StampBehaviour : MonoBehaviour
{
    public bool isCollected;
    Animator stampAnim;
    public GameObject stampGameObject;
    public GameObject historyUI;
    
    /// <summary>
    /// Initializes the Animator component
    /// </summary>
    void Awake()
    {
        stampAnim = this.GetComponent<Animator>();
        
        if (stampAnim == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }
    }

    /// <summary>
    /// Triggers stamp animation when an item is collected
    /// </summary>
    public void ItemCollected()
    {
        if (stampAnim != null)
        {
            stampAnim.SetBool("isStamp", true);
        }
        else
        {
            Debug.LogError("stampAnim is null in ItemCollected!");
        }
    }

    /// <summary>
    /// Deactivates stamp and history UI
    /// </summary>
    public void DeactivateStamp()
    {
        if (stampGameObject != null)
        {
        Debug.Log("Deactivating stamp and history UI");
        stampAnim.SetBool("isStamp", false);
        stampGameObject.SetActive(false);
        historyUI.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}