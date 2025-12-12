using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FinalPuzzleManager : MonoBehaviour
{
    public ARTrackedImageManager imageManager; 
    public string targetImageName = "FinalImage"; // name of reference image

    public GameObject[] modelsToSpawn;  // 5 models
    public Transform spawnParent;

    public GameObject finalModel;
    public Transform finalModelSpawnPoint;

    public DropBox dropBox;


    private bool spawned = false;

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            if (trackedImage.referenceImage.name == targetImageName && !spawned)
            {
                SpawnPuzzleModels(trackedImage.transform);
            }
        }
    }

    void SpawnPuzzleModels(Transform trackedTransform)
    {
        spawned = true;

        foreach (GameObject obj in modelsToSpawn)
        {
            obj.SetActive(true);
            obj.transform.SetParent(spawnParent);
            obj.transform.localPosition = Random.insideUnitSphere * 0.1f;
        }

        dropBox.manager = this;
    }

    public void CheckCompletion()
    {
        if (dropBox.collectedCount == modelsToSpawn.Length)
        {
            finalModel.SetActive(true);
            finalModel.transform.position = finalModelSpawnPoint.position;
        }
    }
}
