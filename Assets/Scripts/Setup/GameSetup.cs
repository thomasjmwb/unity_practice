using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string instructions = @"SETUP INSTRUCTIONS:

1. Create a Canvas for your UI
2. Add this GameSetup script to an empty GameObject
3. Create the following UI structure:
   - Canvas/PlayingUI (contains game controls)
   - Canvas/ShoppingUI (contains shop interface)
   - Canvas/GameOverUI (contains restart button)
   - Canvas/VictoryUI (contains victory message)
   - Canvas/Hand (contains cards in hand)
   - Canvas/Grid (contains the 3x3 grid)

4. Create prefabs for:
   - CardUI (for cards in hand)
   - CardSlot (for grid positions)
   - ShopItemUI (for shop items)

5. Create some Card ScriptableObjects with different effects
6. Assign all references in the components below
7. Press 'Auto Setup Scene' to create basic structure";

    [Header("Prefabs")]
    public GameObject cardUIPrefab;
    public GameObject cardSlotPrefab;
    public GameObject shopItemPrefab;

    [Header("Sample Cards")]
    public Card[] sampleCards;

    [ContextMenu("Auto Setup Scene")]
    public void AutoSetupScene()
    {
        CreateBasicGameStructure();
        Debug.Log("Basic game structure created! Configure the components and create your card assets.");
    }

    void CreateBasicGameStructure()
    {
        // Create main game object with all managers
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();

        // Create Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create UI structure
        CreateUIStructure(canvas.transform);

        // Create Deck
        GameObject deckObj = new GameObject("Deck");
        Deck deck = deckObj.AddComponent<Deck>();
        gameManager.playerDeck = deck;

        // Create Grid
        GameObject gridObj = new GameObject("CardGrid");
        CardGrid grid = gridObj.AddComponent<CardGrid>();
        gameManager.cardGrid = grid;

        // Create Hand Manager
        GameObject handObj = new GameObject("HandManager");
        HandManager handManager = handObj.AddComponent<HandManager>();
        gameManager.handManager = handManager;

        // Create Shop Manager
        GameObject shopObj = new GameObject("ShopManager");
        ShopManager shopManager = shopObj.AddComponent<ShopManager>();
        gameManager.shopManager = shopManager;

        // Create UI Manager
        GameObject uiObj = new GameObject("UIManager");
        UIManager uiManager = uiObj.AddComponent<UIManager>();
        gameManager.uiManager = uiManager;

        Debug.Log("Game structure created! Remember to:");
        Debug.Log("1. Create Card ScriptableObjects");
        Debug.Log("2. Create UI prefabs (CardUI, CardSlot, ShopItemUI)");
        Debug.Log("3. Configure all component references");
        Debug.Log("4. Set up your UI layout");
    }

    void CreateUIStructure(Transform canvasTransform)
    {
        // Create basic UI containers
        CreateUIContainer("PlayingUI", canvasTransform);
        CreateUIContainer("ShoppingUI", canvasTransform);
        CreateUIContainer("GameOverUI", canvasTransform);
        CreateUIContainer("VictoryUI", canvasTransform);
        CreateUIContainer("Hand", canvasTransform);
        CreateUIContainer("Grid", canvasTransform);
    }

    GameObject CreateUIContainer(string name, Transform parent)
    {
        GameObject container = new GameObject(name);
        container.transform.SetParent(parent);
        
        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return container;
    }

    [ContextMenu("Create Sample Cards")]
    public void CreateSampleCards()
    {
        CreateSampleCard("Simple Coin", "Gives 10 gold", CardFaction.Commerce, 1, new SimpleGoldEffect());
        CreateSampleCard("Team Player", "Bonus gold for each adjacent card", CardFaction.Military, 2, new AdjacentBonusEffect());
        CreateSampleCard("Corner Stone", "Bonus gold in corners", CardFaction.Nature, 2, new CornerBonusEffect());
        CreateSampleCard("Faction Leader", "Bonus gold for same faction cards", CardFaction.Magic, 3, new FactionSynergyEffect());
        
        Debug.Log("Sample cards created in Assets folder!");
    }

    void CreateSampleCard(string cardName, string description, CardFaction faction, int cost, CardEffect effect)
    {
        Card newCard = ScriptableObject.CreateInstance<Card>();
        newCard.cardName = cardName;
        newCard.description = description;
        newCard.faction = faction;
        newCard.cost = cost;
        newCard.effects.Add(effect);

        string path = $"Assets/{cardName.Replace(" ", "")}.asset";
        UnityEditor.AssetDatabase.CreateAsset(newCard, path);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}