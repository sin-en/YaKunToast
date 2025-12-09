using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StampBehaviour : MonoBehaviour
{
    public bool isCollected;
    Animator stampAnim;
    public GameObject stampGameObject;
    public GameObject historyUI;
    
    void Awake()
    {
        stampAnim = this.GetComponent<Animator>();
        
        if (stampAnim == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }
    }
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