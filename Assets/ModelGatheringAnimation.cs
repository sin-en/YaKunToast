/*
* Author: Zelda Ng XinYi
* Date: 13/12/2025
* Description: Animates 5 models gathering from a circle to center, then spawns a new model
*/
using System.Collections;
using UnityEngine;
using TMPro;

public class ModelGatheringAnimation : MonoBehaviour
{
    [Header("Model References")]
    public GameObject[] collectibleModels = new GameObject[5];
    public GameObject finalModel;

    [Header("UI References")]
    public TextMeshProUGUI animationText;
    public GameObject congratulationsPanel; // Panel with congratulations message and button

    [Header("Text Settings")]
    public string makingText = "Making Kaya Toast Set...";
    public string readyText = "Kaya Toast Set Ready!";

    [Header("Circle Formation")]
    public float circleRadius = 3f;
    public Vector3 centerPosition = Vector3.zero;

    [Header("Phase 1: Spinning")]
    public float spinDuration = 1f;
    public float spinSpeed = 50f;

    [Header("Phase 2: Gathering")]
    public float gatherDuration = 2f;
    public float gatherSpinSpeed = 100f;

    [Header("Phase 3: Final Model")]
    public float delayBeforeFinalModel = 0.5f;
    public float finalModelAppearDuration = 1f;
    public float finalModelSpinSpeed = 50f;
    public float finalModelHoldDuration = 2f;
    public float finalModelScale = 1f;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "SampleScene 2"; // Change this in Inspector if needed

    private Vector3[] startPositions = new Vector3[5];
    private Vector3[] originalScales = new Vector3[5];

    void Start()
    {
        // Hide text initially
        if (animationText != null)
        {
            animationText.text = "";
        }
        
        // Hide congratulations panel
        if (congratulationsPanel != null)
        {
            congratulationsPanel.SetActive(false);
        }
        
        SetupCirclePositions();
        
        if (finalModel != null)
        {
            finalModel.transform.localScale = Vector3.zero;
            finalModel.SetActive(false);
        }
        
        StartCoroutine(PlayGatheringAnimation());
    }

    void SetupCirclePositions()
    {
        for (int i = 0; i < collectibleModels.Length; i++)
        {
            if (collectibleModels[i] != null)
            {
                // Store original scale
                originalScales[i] = collectibleModels[i].transform.localScale;
                
                float angle = i * (360f / collectibleModels.Length) * Mathf.Deg2Rad;
                Vector3 circlePos = new Vector3(
                    Mathf.Cos(angle) * circleRadius,
                    0f,
                    Mathf.Sin(angle) * circleRadius
                );
                
                collectibleModels[i].transform.position = circlePos + centerPosition;
                startPositions[i] = collectibleModels[i].transform.position;
                collectibleModels[i].transform.LookAt(centerPosition);
            }
        }
    }

    IEnumerator PlayGatheringAnimation()
    {
        float elapsedTime = 0f;

        // Phase 1: Rotate in circle
        while (elapsedTime < spinDuration)
        {
            for (int i = 0; i < collectibleModels.Length; i++)
            {
                if (collectibleModels[i] != null)
                {
                    collectibleModels[i].transform.RotateAround(
                        centerPosition, 
                        Vector3.up, 
                        spinSpeed * Time.deltaTime
                    );
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Update start positions after rotation
        for (int i = 0; i < collectibleModels.Length; i++)
        {
            if (collectibleModels[i] != null)
            {
                startPositions[i] = collectibleModels[i].transform.position;
            }
        }

        // Phase 2: Gather to center
        // Show "Making..." text
        if (animationText != null)
        {
            animationText.text = makingText;
        }
        
        elapsedTime = 0f;
        while (elapsedTime < gatherDuration)
        {
            float t = elapsedTime / gatherDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < collectibleModels.Length; i++)
            {
                if (collectibleModels[i] != null)
                {
                    collectibleModels[i].transform.position = Vector3.Lerp(
                        startPositions[i],
                        centerPosition,
                        smoothT
                    );
                    collectibleModels[i].transform.Rotate(Vector3.up, gatherSpinSpeed * Time.deltaTime);
                    
                    // Shrink based on original scale
                    float scaleFactor = Mathf.Lerp(1f, 0f, smoothT);
                    collectibleModels[i].transform.localScale = originalScales[i] * scaleFactor;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Hide all collected models
        foreach (var model in collectibleModels)
        {
            if (model != null)
                model.SetActive(false);
        }

        // Wait before final model
        yield return new WaitForSeconds(delayBeforeFinalModel);

        // Phase 3: Spawn final model
        if (finalModel != null)
        {
            finalModel.SetActive(true);
            finalModel.transform.position = centerPosition;
            
            // Show "Ready!" text
            if (animationText != null)
            {
                animationText.text = readyText;
            }
            
            // Scale up animation
            elapsedTime = 0f;
            while (elapsedTime < finalModelAppearDuration)
            {
                float t = elapsedTime / finalModelAppearDuration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                
                finalModel.transform.localScale = Vector3.one * finalModelScale * smoothT;
                finalModel.transform.Rotate(Vector3.up, finalModelSpinSpeed * Time.deltaTime);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            finalModel.transform.localScale = Vector3.one * finalModelScale;
            
            // Keep spinning at full size
            elapsedTime = 0f;
            while (elapsedTime < finalModelHoldDuration)
            {
                finalModel.transform.Rotate(Vector3.up, finalModelSpinSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Show congratulations panel
        if (congratulationsPanel != null)
        {
            congratulationsPanel.SetActive(true);
        }
        
        // Hide animation text
        if (animationText != null)
        {
            animationText.text = "";
        }
    }

    /// <summary>
    /// Returns to main menu (call this from button)
    /// </summary>
    public void ReturnToMainMenu()
    {
        Debug.Log($"Attempting to load scene: {mainMenuSceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Call this to restart the animation
    /// </summary>
    public void RestartAnimation()
    {
        StopAllCoroutines();
        
        // Reset models with their original scales
        for (int i = 0; i < collectibleModels.Length; i++)
        {
            if (collectibleModels[i] != null)
            {
                collectibleModels[i].SetActive(true);
                collectibleModels[i].transform.localScale = originalScales[i];
            }
        }

        if (finalModel != null)
        {
            finalModel.SetActive(false);
            finalModel.transform.localScale = Vector3.zero;
        }

        if (animationText != null)
        {
            animationText.text = "";
        }

        SetupCirclePositions();
        StartCoroutine(PlayGatheringAnimation());
    }
}