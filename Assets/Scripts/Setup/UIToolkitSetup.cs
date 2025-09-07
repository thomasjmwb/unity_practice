using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

[System.Serializable]
public class UISetupData
{
    [Header("Required Assets")]
    public VisualTreeAsset mainGameUIAsset;
    public VisualTreeAsset cardUIAsset;
    public StyleSheet gameStyleSheet;
    public StyleSheet cardStyleSheet;
    
    [Header("Auto-Setup Options")]
    public bool createUIDocument = true;
    public bool assignAssets = true;
    public bool setupEventHandlers = true;
}

public class UIToolkitSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    public UISetupData setupData;
    
    [Header("Generated Components")]
    public UIDocument uiDocument;
    public UIToolkitManager uiManager;
    
    [TextArea(10, 15)]
    public string instructions = @"UI TOOLKIT SETUP INSTRUCTIONS:

1. Create the required UXML and USS assets:
   - MainGameUI.uxml (main interface layout)
   - CardUI.uxml (individual card template)  
   - GameStyles.uss (main game styling)
   - CardStyles.uss (card-specific styling)

2. Click 'Auto Setup UI Toolkit' to:
   - Create UIDocument component
   - Assign UXML and USS assets
   - Create UIToolkitManager component
   - Connect event handlers

3. Manual Setup (if needed):
   - Add UIDocument to a GameObject in scene
   - Set Source Asset to MainGameUI.uxml
   - Add UIToolkitManager script
   - Assign all asset references

4. Integration with GameManager:
   - Connect UIToolkitManager to GameManager
   - Update existing UIManager references
   - Test all UI interactions

Assets have been created in:
- /Assets/UI/MainGameUI.uxml
- /Assets/UI/CardUI.uxml
- /Assets/UI/GameStyles.uss
- /Assets/UI/CardStyles.uss";

    [ContextMenu("Auto Setup UI Toolkit")]
    public void AutoSetupUIToolkit()
    {
        SetupUIDocument();
        SetupUIManager();
        LoadAssets();
        ConnectToGameManager();
        
        Debug.Log("UI Toolkit setup complete! Check the console for next steps.");
        LogSetupInstructions();
    }
    
    void SetupUIDocument()
    {
        if (setupData.createUIDocument)
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
            }
            
            Debug.Log("✓ UIDocument component created/found");
        }
    }
    
    void SetupUIManager()
    {
        uiManager = GetComponent<UIToolkitManager>();
        if (uiManager == null)
        {
            uiManager = gameObject.AddComponent<UIToolkitManager>();
        }
        
        uiManager.mainUIDocument = uiDocument;
        Debug.Log("✓ UIToolkitManager component created/found");
    }
    
    void LoadAssets()
    {
        if (!setupData.assignAssets) return;
        
        // Try to load the assets we created
        setupData.mainGameUIAsset = Resources.Load<VisualTreeAsset>("UI/MainGameUI");
        if (setupData.mainGameUIAsset == null)
        {
            Debug.LogWarning("MainGameUI.uxml not found in Resources/UI/. Make sure it's in the correct location.");
        }
        
        setupData.cardUIAsset = Resources.Load<VisualTreeAsset>("UI/CardUI");
        if (setupData.cardUIAsset == null)
        {
            Debug.LogWarning("CardUI.uxml not found in Resources/UI/. Make sure it's in the correct location.");
        }
        
        setupData.gameStyleSheet = Resources.Load<StyleSheet>("UI/GameStyles");
        if (setupData.gameStyleSheet == null)
        {
            Debug.LogWarning("GameStyles.uss not found in Resources/UI/. Make sure it's in the correct location.");
        }
        
        setupData.cardStyleSheet = Resources.Load<StyleSheet>("UI/CardStyles");
        if (setupData.cardStyleSheet == null)
        {
            Debug.LogWarning("CardStyles.uss not found in Resources/UI/. Make sure it's in the correct location.");
        }
        
        // Assign to UIDocument
        if (uiDocument != null && setupData.mainGameUIAsset != null)
        {
            uiDocument.visualTreeAsset = setupData.mainGameUIAsset;
            Debug.Log("✓ Main UI asset assigned to UIDocument");
        }
        
        // Assign to UIManager
        if (uiManager != null)
        {
            uiManager.mainGameUI = setupData.mainGameUIAsset;
            uiManager.cardUI = setupData.cardUIAsset;
            uiManager.gameStyles = setupData.gameStyleSheet;
            uiManager.cardStyles = setupData.cardStyleSheet;
            Debug.Log("✓ Assets assigned to UIToolkitManager");
        }
    }
    
    void ConnectToGameManager()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && uiManager != null)
        {
            // You would replace the old UIManager with UIToolkitManager
            // gameManager.uiManager = uiManager; // This would need a cast or interface
            Debug.Log("✓ Found GameManager - you may need to manually update the reference");
        }
        else
        {
            Debug.LogWarning("GameManager not found. You'll need to connect the UIToolkitManager manually.");
        }
    }
    
    void LogSetupInstructions()
    {
        Debug.Log(@"
NEXT STEPS:

1. Move UXML/USS files to Resources/UI/ folder if you want to load them via Resources.Load()
   OR
   Manually assign them in the inspector

2. Update your GameManager to use UIToolkitManager instead of UIManager

3. Test the UI by running the game:
   - Check that all UI elements display correctly
   - Verify button interactions work
   - Test card dragging and grid placement
   - Confirm shop overlay functions properly

4. Customize the styling in the USS files to match your game's theme

5. Add any additional UI elements you need for your specific game mechanics

The UI Toolkit system is now ready to use!");
    }
    
    [ContextMenu("Create Sample Scene Setup")]
    public void CreateSampleSceneSetup()
    {
        // Create Canvas for traditional UI components (if needed)
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas (Legacy UI)");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1; // Behind UI Toolkit
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create UI Toolkit root
        GameObject uiRoot = new GameObject("UI Toolkit Root");
        uiRoot.AddComponent<UIDocument>();
        uiRoot.AddComponent<UIToolkitManager>();
        
        // Add this setup component
        uiRoot.AddComponent<UIToolkitSetup>();
        
        Debug.Log("Sample scene setup created! Run 'Auto Setup UI Toolkit' on the UI Toolkit Root object.");
    }
    
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        bool isValid = true;
        
        if (uiDocument == null)
        {
            Debug.LogError("❌ UIDocument component missing");
            isValid = false;
        }
        
        if (uiManager == null)
        {
            Debug.LogError("❌ UIToolkitManager component missing");
            isValid = false;
        }
        
        if (setupData.mainGameUIAsset == null)
        {
            Debug.LogError("❌ MainGameUI.uxml asset not assigned");
            isValid = false;
        }
        
        if (setupData.gameStyleSheet == null)
        {
            Debug.LogError("❌ GameStyles.uss asset not assigned");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log("✅ UI Toolkit setup is valid and ready to use!");
        }
        else
        {
            Debug.LogError("❌ UI Toolkit setup has issues. Please fix the errors above.");
        }
    }
}