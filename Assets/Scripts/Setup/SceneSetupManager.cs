using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSetupManager : MonoBehaviour
{
    [Header("Scene Setup")]
    public bool autoSetupOnStart = true;
    
    [Header("Generated Objects")]
    public GameObject gameManagerObj;
    public GameObject uiRootObj;
    public GameObject deckObj;
    public GameObject gridObj;
    public GameObject handObj;
    public GameObject shopObj;
    
    [Header("Sample Cards")]
    public Card[] createdCards;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCompleteScene();
        }
    }
    
    [ContextMenu("Setup Complete Scene")]
    public void SetupCompleteScene()
    {
        Debug.Log("Setting up complete scene...");
        
        CreateGameManager();
        CreateUISystem();
        CreateCards();
        SetupDeck();
        ConnectComponents();
        
        Debug.Log("Scene setup complete! Press Play to test the game.");
    }
    
    void CreateGameManager()
    {
        if (gameManagerObj == null)
        {
            gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            Debug.Log("✓ GameManager created");
        }
    }
    
    void CreateUISystem()
    {
        if (uiRootObj == null)
        {
            uiRootObj = new GameObject("UI Root");
            
            // Add UI Toolkit test only
            UIDocument uiDocument = uiRootObj.AddComponent<UIDocument>();
            
            // Create Panel Settings asset for Unity 6 UI Toolkit
            PanelSettings panelSettings = CreatePanelSettings();
            uiDocument.panelSettings = panelSettings;
            
            SimpleUITest simpleTest = uiRootObj.AddComponent<SimpleUITest>();
            
            // Load UI assets from the correct paths
#if UNITY_EDITOR
            var mainUIAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/MainGameUI.uxml");
            var gameStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/GameStyles.uss");
            var cardStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/CardStyles.uss");
#else
            var mainUIAsset = Resources.Load<VisualTreeAsset>("UI/MainGameUI");
            var gameStyles = Resources.Load<StyleSheet>("UI/GameStyles");
            var cardStyles = Resources.Load<StyleSheet>("UI/CardStyles");
#endif
            
            // TEMPORARILY DISABLE COMPLEX UI SYSTEM - using simple test instead
            bool useSimpleTestUI = true;
            
            if (!useSimpleTestUI && mainUIAsset != null)
            {
                uiDocument.visualTreeAsset = mainUIAsset;
                
                // Add UI Toolkit Manager only when using full UI
                UIToolkitManager uiManager = uiRootObj.AddComponent<UIToolkitManager>();
                uiManager.mainUIDocument = uiDocument;
                
                // Assign style sheets
                if (gameStyles != null)
                {
                    uiManager.gameStyles = gameStyles;
                    Debug.Log("✓ Game styles loaded");
                }
                
                if (cardStyles != null)
                {
                    uiManager.cardStyles = cardStyles;
                    Debug.Log("✓ Card styles loaded");
                }
                
                // Also add legacy UI Manager for compatibility
                UIManager legacyUIManager = uiRootObj.AddComponent<UIManager>();
                legacyUIManager.useUIToolkit = true;
                legacyUIManager.uiToolkitManager = uiManager;
                
                Debug.Log("✓ Full UI System created");
            }
            else
            {
                Debug.Log("✓ UI Toolkit test created with Panel Settings");
            }
        }
    }
    
    PanelSettings CreatePanelSettings()
    {
        // Create a PanelSettings asset at runtime
        PanelSettings panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        
        // Configure panel settings for proper UI Toolkit rendering
        panelSettings.referenceResolution = new Vector2Int(1920, 1080);
        panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
        panelSettings.match = 0.5f; // Balance between width and height matching
        panelSettings.sortingOrder = 0;
        
        // Set theme style sheet to null (we'll use our own styles)
        panelSettings.themeStyleSheet = null;
        
#if UNITY_EDITOR
        // Save the panel settings asset for future use
        if (!AssetDatabase.IsValidFolder("Assets/UI"))
        {
            AssetDatabase.CreateFolder("Assets", "UI");
        }
        
        string assetPath = "Assets/UI/DefaultPanelSettings.asset";
        if (!System.IO.File.Exists(assetPath))
        {
            AssetDatabase.CreateAsset(panelSettings, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("✓ Panel Settings asset created at: " + assetPath);
        }
#endif
        
        Debug.Log("✓ Panel Settings configured for UI Toolkit");
        return panelSettings;
    }
    
    void CreateCards()
    {
        createdCards = new Card[10];
        
        // Create 10 different types of cards with various effects
        createdCards[0] = CreateCard("Gold Coin", "Simple gold generation", CardFaction.Commerce, 1, 1, typeof(SimpleGoldEffect), 10);
        createdCards[1] = CreateCard("Market Stall", "More gold near other cards", CardFaction.Commerce, 2, 1, typeof(AdjacentBonusEffect), 5);
        createdCards[2] = CreateCard("Watchtower", "Strong in corners", CardFaction.Military, 3, 2, typeof(CornerBonusEffect), 8);
        createdCards[3] = CreateCard("Tree Grove", "Nature synergy bonus", CardFaction.Nature, 2, 2, typeof(FactionSynergyEffect), 6);
        createdCards[4] = CreateCard("Crystal Mine", "Escalates each round", CardFaction.Magic, 4, 3, typeof(EscalatingEffect), 3);
        createdCards[5] = CreateCard("Trade Route", "Forms powerful lines", CardFaction.Commerce, 3, 2, typeof(LineFormationEffect), 5);
        createdCards[6] = CreateCard("Tech Hub", "Technology advancement", CardFaction.Technology, 3, 2, typeof(FactionSynergyEffect), 7);
        createdCards[7] = CreateCard("Fortress", "Defensive position", CardFaction.Military, 4, 3, typeof(CornerBonusEffect), 12);
        createdCards[8] = CreateCard("Mystic Circle", "Magic amplification", CardFaction.Magic, 2, 2, typeof(AdjacentBonusEffect), 8);
        createdCards[9] = CreateCard("Forest Heart", "Nature's bounty", CardFaction.Nature, 5, 3, typeof(EscalatingEffect), 5);
        
        Debug.Log("✓ 10 card types created");
    }
    
    Card CreateCard(string name, string description, CardFaction faction, int cost, int rarity, System.Type effectType, int effectValue)
    {
        Card card = ScriptableObject.CreateInstance<Card>();
        card.cardName = name;
        card.description = description;
        card.faction = faction;
        card.cost = cost;
        card.rarity = rarity;
        
        // Create and add the effect using constructor
        CardEffect effect = new CardEffect();
        effect.effectName = $"{card.cardName} Effect";
        effect.baseValue = effectValue;
        effect.requiresPosition = false;
        effect.requiresFactionCount = false;
        effect.isGlobalModifier = false;
        card.effects.Add(effect);
        
        // Set positioning preferences based on card type
        switch (effectType.Name)
        {
            case "CornerBonusEffect":
                card.prefersCorners = true;
                break;
            case "AdjacentBonusEffect":
                card.prefersEdges = true;
                break;
            case "LineFormationEffect":
                card.prefersEdges = true;
                break;
        }
        
        // Save as asset
        string path = $"Assets/Cards/{name.Replace(" ", "")}.asset";
        UnityEditor.AssetDatabase.CreateAsset(card, path);
        
        return card;
    }
    
    void SetupDeck()
    {
        if (deckObj == null)
        {
            deckObj = new GameObject("PlayerDeck");
            Deck deck = deckObj.AddComponent<Deck>();
            
            // Add 3 copies of each card to the starting deck
            deck.startingCards.Clear();
            foreach (Card card in createdCards)
            {
                if (card != null)
                {
                    deck.startingCards.Add(new DeckEntry(card, 3));
                }
            }
            
            Debug.Log("✓ Deck created with 30 cards (3 copies of each)");
        }
    }
    
    void ConnectComponents()
    {
        // Get components
        GameManager gameManager = gameManagerObj?.GetComponent<GameManager>();
        Deck deck = deckObj?.GetComponent<Deck>();
        UIManager uiManager = uiRootObj?.GetComponent<UIManager>();
        UIToolkitManager uiToolkitManager = uiRootObj?.GetComponent<UIToolkitManager>();
        
        if (gameManager == null) 
        {
            Debug.LogError("GameManager component not found!");
            return;
        }
        
        // Create other required components
        if (gridObj == null)
        {
            gridObj = new GameObject("CardGrid");
            CardGrid grid = gridObj.AddComponent<CardGrid>();
            gameManager.cardGrid = grid;
            
            // Position the grid in world space
            gridObj.transform.position = new Vector3(0, 0, 0);
        }
        
        if (handObj == null)
        {
            handObj = new GameObject("HandManager");
            HandManager handManager = handObj.AddComponent<HandManager>();
            gameManager.handManager = handManager;
        }
        
        if (shopObj == null)
        {
            shopObj = new GameObject("ShopManager");
            ShopManager shopManager = shopObj.AddComponent<ShopManager>();
            
            // Add all created cards to the shop's available cards
            if (createdCards != null)
            {
                shopManager.availableCards.AddRange(createdCards);
                shopManager.tier1Cards.AddRange(System.Array.FindAll(createdCards, c => c.rarity <= 1));
                shopManager.tier2Cards.AddRange(System.Array.FindAll(createdCards, c => c.rarity == 2));
                shopManager.tier3Cards.AddRange(System.Array.FindAll(createdCards, c => c.rarity >= 3));
            }
            
            gameManager.shopManager = shopManager;
        }
        
        // Connect all references
        gameManager.playerDeck = deck;
        gameManager.uiManager = uiManager;
        
        // Initialize the game immediately
        Debug.Log("🎮 Starting GameManager initialization...");
        gameManager.InitializeGame();
        
        Debug.Log("✓ All components connected");
    }
    
    [ContextMenu("Create Card Prefabs")]
    public void CreateCardPrefabs()
    {
        CreateCardUIPrefab();
        CreateCardSlotPrefab();
        CreateShopItemPrefab();
    }
    
    void CreateCardUIPrefab()
    {
        GameObject cardUIPrefab = new GameObject("CardUI");
        
        // Add Canvas for UI elements
        Canvas canvas = cardUIPrefab.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        cardUIPrefab.AddComponent<CanvasGroup>();
        cardUIPrefab.AddComponent<CardUI>();
        
        // Save as prefab
        string path = "Assets/Prefabs/CardUI.prefab";
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(cardUIPrefab, path);
        
        DestroyImmediate(cardUIPrefab);
        Debug.Log("✓ CardUI prefab created");
    }
    
    void CreateCardSlotPrefab()
    {
        GameObject slotPrefab = new GameObject("CardSlot");
        
        // Add visual components
        slotPrefab.AddComponent<UnityEngine.UI.Image>();
        slotPrefab.AddComponent<CardSlot>();
        
        // Save as prefab
        string path = "Assets/Prefabs/CardSlot.prefab";
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(slotPrefab, path);
        
        DestroyImmediate(slotPrefab);
        Debug.Log("✓ CardSlot prefab created");
    }
    
    void CreateShopItemPrefab()
    {
        GameObject shopItemPrefab = new GameObject("ShopItemUI");
        
        shopItemPrefab.AddComponent<UnityEngine.UI.Image>();
        shopItemPrefab.AddComponent<ShopItemUI>();
        
        // Save as prefab
        string path = "Assets/Prefabs/ShopItemUI.prefab";
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(shopItemPrefab, path);
        
        DestroyImmediate(shopItemPrefab);
        Debug.Log("✓ ShopItemUI prefab created");
    }
}