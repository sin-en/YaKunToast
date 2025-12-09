/*
* Author: Kwek Sin En
* Date: 15/11/2025
* Description: Manages UI screens such as login and registration
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    //Screen object variables
    public GameObject loginUI;
    public GameObject signUpUI;

    /// <summary>
    /// Initializes singleton instance of UIManager.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    /// <summary>
    /// Switches to the login screen UI.
    /// </summary>
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        signUpUI.SetActive(false);
    }
    /// <summary>
    /// Switches to the registration screen UI.
    /// </summary>
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        signUpUI.SetActive(true);
    }
}