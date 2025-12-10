/*
* Author: Kwek Sin En
* Date: 26/11/2025
* Description: Handles scene loading functionalities
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    public int sceneIndex;
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
