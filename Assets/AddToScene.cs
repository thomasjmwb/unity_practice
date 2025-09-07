using UnityEngine;

// This script creates a GameObject with the CompleteGameSetup component in the current scene
// Run this by selecting the script and clicking "Add Game Setup To Scene" in the inspector

public class AddToScene : MonoBehaviour
{
    [Header("Scene Setup")]
    [Tooltip("Click this button to add the game setup to the current scene")]
    public bool addGameSetupToScene = false;
    
    void OnValidate()
    {
        if (addGameSetupToScene)
        {
            addGameSetupToScene = false;
            AddGameSetupToCurrentScene();
        }
    }
    
    [ContextMenu("Add Game Setup To Scene")]
    public void AddGameSetupToCurrentScene()
    {
        // Check if setup already exists
        if (FindObjectOfType<WorkingGameSetup>() != null)
        {
            Debug.Log("⚠️ Game setup already exists in scene");
            return;
        }
        
        // Create new GameObject with the setup component
        GameObject setupObj = new GameObject("🎮 Game Setup Manager");
        WorkingGameSetup setup = setupObj.AddComponent<WorkingGameSetup>();
        
        // Position it near the camera for visibility
        setupObj.transform.position = new Vector3(0, 2, 0);
        
        Debug.Log("✅ Game setup added to scene! The game will be configured automatically.");
        Debug.Log("🎯 What will be created:");
        Debug.Log("   • GameManager with all settings");
        Debug.Log("   • 10 different card types (30 total cards)");
        Debug.Log("   • Complete UI system with modern styling");
        Debug.Log("   • Card grid, hand manager, and shop system");
        Debug.Log("   • All components connected and ready to play");
        
        // Remove this script as it's no longer needed
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
}