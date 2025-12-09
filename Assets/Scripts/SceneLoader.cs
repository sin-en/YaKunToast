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
