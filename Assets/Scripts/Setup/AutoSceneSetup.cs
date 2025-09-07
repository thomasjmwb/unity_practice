using UnityEngine;

[System.Serializable]
public class AutoSceneSetup : MonoBehaviour
{
    [Header("Scene Auto-Setup")]
    [SerializeField] private bool hasSetup = false;
    
    void Awake()
    {
        if (!hasSetup)
        {
            SetupScene();
            hasSetup = true;
        }
    }
    
    void SetupScene()
    {
        Debug.Log("🎮 Auto-setting up Landlord Card Game scene...");
        
        // Add QuickSceneSetup component and run setup
        QuickSceneSetup setup = gameObject.AddComponent<QuickSceneSetup>();
        setup.SetupGame();
        
        // Remove this component after setup
        Destroy(this);
    }
}