using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MenuBehaviour : MonoBehaviour
{
    /// <summary>
    /// The build index of the target scene to load.
    /// </summary>
    [SerializeField]
    int targetSceneIndex = 1;

    /// <summary>
    /// Press Sign Up and Login button to load the target scene.
    /// </summary>
    public void SignIn()
    {
        SceneManager.LoadSceneAsync(targetSceneIndex);
    }
}
