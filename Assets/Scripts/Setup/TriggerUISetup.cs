using UnityEngine;

public class TriggerUISetup : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TriggerUISetup started. Press Space to manually trigger UI setup, or it should happen automatically.");
    }
    
    void Update()
    {
        // Press Space to manually trigger setup
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSetup();
        }
    }
    
    public void TriggerSetup()
    {
        // Find and trigger the scene setup manager
        SceneSetupManager setupManager = FindFirstObjectByType<SceneSetupManager>();
        if (setupManager != null)
        {
            Debug.Log("🎮 Manually triggering complete scene setup...");
            setupManager.SetupCompleteScene();
            
            // Check what was created
            CheckUISetup();
        }
        else
        {
            Debug.LogError("SceneSetupManager not found!");
        }
    }
    
    void CheckUISetup()
    {
        // Check if UI system was created
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        UIToolkitManager uiToolkitManager = FindFirstObjectByType<UIToolkitManager>();
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        
        Debug.Log($"UI Setup Check:");
        Debug.Log($"  - UIManager: {uiManager != null}");
        Debug.Log($"  - UIToolkitManager: {uiToolkitManager != null}");
        Debug.Log($"  - GameManager: {gameManager != null}");
        
        if (uiToolkitManager != null && uiToolkitManager.mainUIDocument != null)
        {
            Debug.Log($"  - UIDocument: {uiToolkitManager.mainUIDocument != null}");
            Debug.Log($"  - Panel Settings: {uiToolkitManager.mainUIDocument.panelSettings != null}");
            Debug.Log($"  - Visual Tree Asset: {uiToolkitManager.mainUIDocument.visualTreeAsset != null}");
        }
    }
}