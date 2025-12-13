using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
    public static DebugDisplay Instance;
    
    private Text debugText;
    private string debugLog = "";
    private int maxLines = 25; // Show more lines with smaller text

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateDebugCanvas();
        }
    }

    void CreateDebugCanvas()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("DebugCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Text
        GameObject textObj = new GameObject("DebugText");
        textObj.transform.SetParent(canvasObj.transform);
        
        debugText = textObj.AddComponent<Text>();
        debugText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        debugText.fontSize = 14; // Smaller font size
        debugText.color = Color.white;
        debugText.alignment = TextAnchor.UpperLeft;
        
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(10, 10);
        rectTransform.offsetMax = new Vector2(-10, -10);
        
        // Add black background for readability
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(textObj.transform);
        bgObj.transform.SetAsFirstSibling();
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
    }

    public static void Log(string message)
    {
        if (Instance != null)
        {
            Instance.AddLog(message);
        }
    }

    void AddLog(string message)
    {
        debugLog += message + "\n";
        
        // Keep only last N lines
        string[] lines = debugLog.Split('\n');
        if (lines.Length > maxLines)
        {
            debugLog = string.Join("\n", lines, lines.Length - maxLines, maxLines);
        }
        
        if (debugText != null)
        {
            debugText.text = debugLog;
        }
    }
}