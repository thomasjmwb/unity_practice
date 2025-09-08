using UnityEngine;
using UnityEngine.UIElements;

public class SimpleUITest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("SimpleUITest: Starting...");
        
        // Get UIDocument (should already exist from SceneSetupManager)
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("SimpleUITest: No UIDocument found! SceneSetupManager should have created one.");
            return;
        }
        
        // Check Panel Settings (should be assigned by SceneSetupManager)
        if (uiDocument.panelSettings == null)
        {
            Debug.LogError("SimpleUITest: No Panel Settings assigned! UI Toolkit won't render.");
            return;
        }
        else
        {
            Debug.Log($"SimpleUITest: Panel Settings assigned: {uiDocument.panelSettings.name}");
        }
        
        Debug.Log($"SimpleUITest: UIDocument found, root has {uiDocument.rootVisualElement.childCount} children");
        
        // Create our test UI
        CreateSimpleTestUI(uiDocument.rootVisualElement);
    }
    
    void CreateSimpleTestUI(VisualElement root)
    {
        Debug.Log("SimpleUITest: Creating test UI...");
        
        // Clear any existing content
        root.Clear();
        
        // Set the root to fill the screen
        root.style.width = Length.Percent(100);
        root.style.height = Length.Percent(100);
        root.style.position = Position.Absolute;
        root.style.top = 0;
        root.style.left = 0;
        
        // Create a main container with visible styling
        VisualElement mainContainer = new VisualElement();
        mainContainer.style.backgroundColor = new Color(0.1f, 0.2f, 0.8f, 1f); // Bright blue background
        mainContainer.style.width = Length.Percent(100);
        mainContainer.style.height = Length.Percent(100);
        mainContainer.style.position = Position.Absolute;
        mainContainer.style.top = 0;
        mainContainer.style.left = 0;
        mainContainer.style.flexDirection = FlexDirection.Column;
        mainContainer.style.justifyContent = Justify.Center;
        mainContainer.style.alignItems = Align.Center;
        
        // Add a big visible label
        Label testLabel = new Label("🎮 UI TOOLKIT WORKS!");
        testLabel.style.fontSize = 60;
        testLabel.style.color = Color.white;
        testLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        testLabel.style.marginBottom = 30;
        testLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        // Add a bright red box for maximum visibility
        VisualElement colorBox = new VisualElement();
        colorBox.style.width = 300;
        colorBox.style.height = 300;
        colorBox.style.backgroundColor = new Color(1f, 0.1f, 0.1f, 1f); // Bright red
        colorBox.style.borderTopLeftRadius = 20;
        colorBox.style.borderTopRightRadius = 20;
        colorBox.style.borderBottomLeftRadius = 20;
        colorBox.style.borderBottomRightRadius = 20;
        colorBox.style.marginBottom = 30;
        
        // Add some info text
        Label infoLabel = new Label("THIS IS THE SIMPLE UI TEST");
        infoLabel.style.fontSize = 24;
        infoLabel.style.color = Color.yellow;
        infoLabel.style.marginBottom = 30;
        infoLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        infoLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        // Add a test button
        Button testButton = new Button();
        testButton.text = "CLICK TO TEST";
        testButton.style.fontSize = 24;
        testButton.style.width = 200;
        testButton.style.height = 60;
        testButton.style.backgroundColor = new Color(0.1f, 0.8f, 0.1f, 1f); // Bright green
        testButton.style.color = Color.white;
        testButton.style.unityFontStyleAndWeight = FontStyle.Bold;
        testButton.clicked += () => {
            Debug.Log("SimpleUITest: Button clicked! UI is working!");
            testLabel.text = "✅ BUTTON WORKS!";
            testLabel.style.color = Color.green;
        };
        
        // Add everything to the main container
        mainContainer.Add(testLabel);
        mainContainer.Add(colorBox);
        mainContainer.Add(infoLabel);
        mainContainer.Add(testButton);
        
        // Add main container to root
        root.Add(mainContainer);
        
        Debug.Log("SimpleUITest: Test UI created successfully!");
        Debug.Log($"SimpleUITest: Root now has {root.childCount} children");
        Debug.Log($"SimpleUITest: Main container has {mainContainer.childCount} children");
    }
}