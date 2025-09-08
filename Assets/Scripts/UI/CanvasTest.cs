using UnityEngine;
using UnityEngine.UI;

public class CanvasTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("CanvasTest: Creating fallback Canvas UI...");
        
        // Create Canvas
        GameObject canvasGO = new GameObject("TestCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // Make sure it's on top
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create a bright colored panel
        GameObject panelGO = new GameObject("TestPanel");
        panelGO.transform.SetParent(canvasGO.transform);
        
        Image panel = panelGO.AddComponent<Image>();
        panel.color = Color.magenta; // Bright magenta - impossible to miss
        
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create text
        GameObject textGO = new GameObject("TestText");
        textGO.transform.SetParent(panelGO.transform);
        
        Text text = textGO.AddComponent<Text>();
        text.text = "LEGACY CANVAS UI WORKS!\n\nIf you see this, Unity's UI system is functional.\nThe issue is with UI Toolkit specifically.";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 48;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(50, 50);
        textRect.offsetMax = new Vector2(-50, -50);
        
        Debug.Log("CanvasTest: Canvas UI created successfully!");
    }
}