using UnityEngine;
using UnityEngine.UIElements;

public class QuickSceneSetup : MonoBehaviour
{
    [Header("Quick Setup")]
    [Tooltip("Click this button to set up the entire game in the current scene")]
    public bool setupScene = false;
    
    void Start()
    {
        // Automatically set up the scene when the game starts
        SetupGame();
    }
    
    void OnValidate()
    {
        if (setupScene)
        {
            setupScene = false;
            SetupGame();
        }
    }
    
    public void SetupGame()
    {
        Debug.Log("🎮 Setting up Landlord Card Game...");
        
        // 1. Create Game Manager
        CreateGameManager();
        
        // 2. Create UI System
        CreateUISystem();
        
        // 3. Create Sample Cards
        CreateSampleCards();
        
        // 4. Setup Game Components
        SetupGameComponents();
        
        Debug.Log("✅ Game setup complete! Press Play to start the game.");
    }
    
    void CreateGameManager()
    {
        if (FindObjectOfType<GameManager>() != null) return;
        
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
        
        // Set some default values
        gameManager.cardsPerTurn = 2;
        gameManager.initialGoldTarget = 100;
        gameManager.goldTargetMultiplier = 1.5f;
        gameManager.maxRounds = 12;
        
        Debug.Log("✓ GameManager created");
    }
    
    void CreateUISystem()
    {
        if (FindObjectOfType<UIDocument>() != null) return;
        
        // Create UI Document GameObject
        GameObject uiRoot = new GameObject("UI Toolkit Root");
        UIDocument uiDocument = uiRoot.AddComponent<UIDocument>();
        
        // Create the UXML programmatically since we can't load from Resources in build
        CreateUIXMLProgrammatically(uiDocument);
        
        // Add UI Toolkit Manager
        UIToolkitManager uiToolkitManager = uiRoot.AddComponent<UIToolkitManager>();
        uiToolkitManager.mainUIDocument = uiDocument;
        
        // Add legacy UI Manager for compatibility
        UIManager uiManager = uiRoot.AddComponent<UIManager>();
        uiManager.useUIToolkit = true;
        uiManager.uiToolkitManager = uiToolkitManager;
        
        Debug.Log("✓ UI System created");
    }
    
    void CreateUIXMLProgrammatically(UIDocument uiDocument)
    {
        // Create a simple UI programmatically
        var root = uiDocument.rootVisualElement;
        root.style.flexGrow = 1;
        root.style.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        
        // Top bar
        var topBar = new VisualElement();
        topBar.style.flexDirection = FlexDirection.Row;
        topBar.style.justifyContent = Justify.SpaceBetween;
        topBar.style.paddingLeft = 15;
        topBar.style.paddingRight = 15;
        topBar.style.paddingTop = 15;
        topBar.style.paddingBottom = 15;
        topBar.style.backgroundColor = new Color(0.15f, 0.2f, 0.25f, 1f);
        topBar.style.minHeight = 80;
        
        // Game info panels
        topBar.Add(CreateInfoPanel("Round", "1", "round-value"));
        topBar.Add(CreateInfoPanel("Gold", "0", "gold-value"));
        topBar.Add(CreateInfoPanel("Target", "100", "target-value"));
        topBar.Add(CreateInfoPanel("Deck", "30", "deck-value"));
        topBar.Add(CreateInfoPanel("Playing", "PLAYING", "phase-value"));
        
        root.Add(topBar);
        
        // Main game area
        var gameArea = new VisualElement();
        gameArea.style.flexGrow = 1;
        gameArea.style.flexDirection = FlexDirection.Row;
        gameArea.style.paddingLeft = 20;
        gameArea.style.paddingRight = 20;
        gameArea.style.paddingTop = 20;
        gameArea.style.paddingBottom = 20;
        
        // Grid area
        var gridContainer = new VisualElement();
        gridContainer.style.flexGrow = 2;
        gridContainer.style.alignItems = Align.Center;
        gridContainer.style.justifyContent = Justify.Center;
        
        var gridTitle = new Label("Card Grid");
        gridTitle.style.fontSize = 18;
        gridTitle.style.color = Color.white;
        gridTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
        gridTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        gridTitle.style.marginBottom = 15;
        gridContainer.Add(gridTitle);
        
        var cardGrid = new VisualElement();
        cardGrid.name = "card-grid";
        cardGrid.style.width = 360;
        cardGrid.style.height = 360;
        cardGrid.style.flexDirection = FlexDirection.Row;
        cardGrid.style.flexWrap = Wrap.Wrap;
        cardGrid.style.backgroundColor = new Color(0.1f, 0.15f, 0.2f, 1f);
        cardGrid.style.borderTopLeftRadius = 12;
        cardGrid.style.borderTopRightRadius = 12;
        cardGrid.style.borderBottomLeftRadius = 12;
        cardGrid.style.borderBottomRightRadius = 12;
        cardGrid.style.paddingLeft = 10;
        cardGrid.style.paddingRight = 10;
        cardGrid.style.paddingTop = 10;
        cardGrid.style.paddingBottom = 10;
        
        // Create 9 grid slots
        for (int i = 0; i < 9; i++)
        {
            var slot = new VisualElement();
            slot.name = $"slot-{i % 3}-{i / 3}";
            slot.style.width = 110;
            slot.style.height = 110;
            slot.style.backgroundColor = new Color(0.2f, 0.25f, 0.3f, 1f);
            slot.style.borderTopLeftRadius = 8;
            slot.style.borderTopRightRadius = 8;
            slot.style.borderBottomLeftRadius = 8;
            slot.style.borderBottomRightRadius = 8;
            slot.style.borderLeftWidth = 2;
            slot.style.borderRightWidth = 2;
            slot.style.borderTopWidth = 2;
            slot.style.borderBottomWidth = 2;
            slot.style.borderLeftColor = new Color(0.3f, 0.4f, 0.5f, 1f);
            slot.style.borderRightColor = new Color(0.3f, 0.4f, 0.5f, 1f);
            slot.style.borderTopColor = new Color(0.3f, 0.4f, 0.5f, 1f);
            slot.style.borderBottomColor = new Color(0.3f, 0.4f, 0.5f, 1f);
            slot.style.marginLeft = 2;
            slot.style.marginRight = 2;
            slot.style.marginTop = 2;
            slot.style.marginBottom = 2;
            cardGrid.Add(slot);
        }
        
        gridContainer.Add(cardGrid);
        
        var gridInfo = new Label("Total Gold: 0");
        gridInfo.name = "grid-gold-total";
        gridInfo.style.fontSize = 20;
        gridInfo.style.color = new Color(1f, 0.84f, 0f, 1f); // Gold color
        gridInfo.style.unityFontStyleAndWeight = FontStyle.Bold;
        gridInfo.style.unityTextAlign = TextAnchor.MiddleCenter;
        gridInfo.style.marginTop = 15;
        gridContainer.Add(gridInfo);
        
        gameArea.Add(gridContainer);
        
        // Hand area
        var handContainer = new VisualElement();
        handContainer.style.flexGrow = 1;
        handContainer.style.minWidth = 300;
        handContainer.style.marginLeft = 30;
        
        var handTitle = new Label("Hand");
        handTitle.style.fontSize = 18;
        handTitle.style.color = Color.white;
        handTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
        handTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        handTitle.style.marginBottom = 15;
        handContainer.Add(handTitle);
        
        var handScroll = new ScrollView();
        handScroll.name = "hand-scroll";
        handScroll.style.flexGrow = 1;
        handScroll.style.backgroundColor = new Color(0.1f, 0.15f, 0.2f, 1f);
        handScroll.style.borderTopLeftRadius = 12;
        handScroll.style.borderTopRightRadius = 12;
        handScroll.style.borderBottomLeftRadius = 12;
        handScroll.style.borderBottomRightRadius = 12;
        handScroll.style.paddingLeft = 15;
        handScroll.style.paddingRight = 15;
        handScroll.style.paddingTop = 15;
        handScroll.style.paddingBottom = 15;
        
        var handCards = new VisualElement();
        handCards.name = "hand-cards";
        handCards.style.flexDirection = FlexDirection.Column;
        handScroll.Add(handCards);
        
        handContainer.Add(handScroll);
        gameArea.Add(handContainer);
        
        root.Add(gameArea);
        
        // Bottom bar
        var bottomBar = new VisualElement();
        bottomBar.style.flexDirection = FlexDirection.Row;
        bottomBar.style.justifyContent = Justify.Center;
        bottomBar.style.alignItems = Align.Center;
        bottomBar.style.paddingLeft = 15;
        bottomBar.style.paddingRight = 15;
        bottomBar.style.paddingTop = 15;
        bottomBar.style.paddingBottom = 15;
        bottomBar.style.backgroundColor = new Color(0.15f, 0.2f, 0.25f, 1f);
        
        var endRoundBtn = new Button(() => {
            GameManager.Instance?.EndRound();
        });
        endRoundBtn.name = "end-round-btn";
        endRoundBtn.text = "End Round";
        endRoundBtn.style.marginRight = 15;
        StyleButton(endRoundBtn, true);
        
        var autoPlaceBtn = new Button(() => {
            Debug.Log("Auto Place clicked");
        });
        autoPlaceBtn.name = "auto-place-btn";
        autoPlaceBtn.text = "Auto Place";
        autoPlaceBtn.style.marginRight = 15;
        StyleButton(autoPlaceBtn, false);
        
        var clearGridBtn = new Button(() => {
            FindObjectOfType<CardGrid>()?.ClearGrid();
        });
        clearGridBtn.name = "clear-grid-btn";
        clearGridBtn.text = "Clear Grid";
        StyleButton(clearGridBtn, false);
        
        bottomBar.Add(endRoundBtn);
        bottomBar.Add(autoPlaceBtn);
        bottomBar.Add(clearGridBtn);
        
        root.Add(bottomBar);
        
        Debug.Log("✓ UI created programmatically");
    }
    
    VisualElement CreateInfoPanel(string label, string value, string valueName)
    {
        var panel = new VisualElement();
        panel.style.flexDirection = FlexDirection.Column;
        panel.style.alignItems = Align.Center;
        panel.style.paddingLeft = 10;
        panel.style.paddingRight = 10;
        panel.style.paddingTop = 10;
        panel.style.paddingBottom = 10;
        panel.style.backgroundColor = new Color(0.2f, 0.25f, 0.3f, 1f);
        panel.style.borderTopLeftRadius = 8;
        panel.style.borderTopRightRadius = 8;
        panel.style.borderBottomLeftRadius = 8;
        panel.style.borderBottomRightRadius = 8;
        panel.style.minWidth = 100;
        
        var labelElement = new Label(label);
        labelElement.style.fontSize = 12;
        labelElement.style.color = new Color(0.7f, 0.8f, 0.9f, 1f);
        labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;
        labelElement.style.marginBottom = 5;
        
        var valueElement = new Label(value);
        valueElement.name = valueName;
        valueElement.style.fontSize = 20;
        valueElement.style.color = Color.white;
        valueElement.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        panel.Add(labelElement);
        panel.Add(valueElement);
        
        return panel;
    }
    
    void StyleButton(Button button, bool isPrimary)
    {
        button.style.paddingTop = 12;
        button.style.paddingBottom = 12;
        button.style.paddingLeft = 24;
        button.style.paddingRight = 24;
        button.style.borderTopLeftRadius = 6;
        button.style.borderTopRightRadius = 6;
        button.style.borderBottomLeftRadius = 6;
        button.style.borderBottomRightRadius = 6;
        button.style.fontSize = 14;
        button.style.unityFontStyleAndWeight = FontStyle.Bold;
        button.style.minWidth = 120;
        
        if (isPrimary)
        {
            button.style.backgroundColor = new Color(0.27f, 0.51f, 0.78f, 1f);
            button.style.color = Color.white;
        }
        else
        {
            button.style.backgroundColor = new Color(0.23f, 0.27f, 0.33f, 1f);
            button.style.color = new Color(0.78f, 0.82f, 0.86f, 1f);
        }
    }
    
    void CreateSampleCards()
    {
        // Create card directory if it doesn't exist
        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Cards"))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Cards");
        }
        
        // Check if cards already exist
        string[] existingCards = UnityEditor.AssetDatabase.FindAssets("t:Card", new[] { "Assets/Cards" });
        if (existingCards.Length >= 10)
        {
            Debug.Log("✓ Cards already exist");
            return;
        }
        
        // Create 10 different card types
        CreateCard("Gold Coin", "Generates 10 gold", CardFaction.Commerce, 1, 1, 10);
        CreateCard("Market Stall", "Bonus for adjacent cards", CardFaction.Commerce, 2, 1, 5);
        CreateCard("Watchtower", "Strong in corners", CardFaction.Military, 3, 2, 8);
        CreateCard("Tree Grove", "Nature synergy bonus", CardFaction.Nature, 2, 2, 6);
        CreateCard("Crystal Mine", "Grows stronger each round", CardFaction.Magic, 4, 3, 3);
        CreateCard("Trade Route", "Forms powerful lines", CardFaction.Commerce, 3, 2, 5);
        CreateCard("Tech Hub", "Technology advancement", CardFaction.Technology, 3, 2, 7);
        CreateCard("Fortress", "Defensive position", CardFaction.Military, 4, 3, 12);
        CreateCard("Mystic Circle", "Magic amplification", CardFaction.Magic, 2, 2, 8);
        CreateCard("Forest Heart", "Nature's growing power", CardFaction.Nature, 5, 3, 5);
        
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        
        Debug.Log("✓ 10 card types created");
    }
    
    void CreateCard(string name, string description, CardFaction faction, int cost, int rarity, int goldValue)
    {
        Card card = ScriptableObject.CreateInstance<Card>();
        card.cardName = name;
        card.description = description;
        card.faction = faction;
        card.cost = cost;
        card.rarity = rarity;
        
        // Create simple gold effect using constructor
        SimpleGoldEffect effect = new SimpleGoldEffect();
        effect.effectName = "Gold Generation";
        effect.baseValue = goldValue;
        card.effects.Add(effect);
        
        string path = $"Assets/Cards/{name.Replace(" ", "")}.asset";
        UnityEditor.AssetDatabase.CreateAsset(card, path);
    }
    
    void SetupGameComponents()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) return;
        
        // Create Deck
        if (gameManager.playerDeck == null)
        {
            GameObject deckObj = new GameObject("PlayerDeck");
            Deck deck = deckObj.AddComponent<Deck>();
            gameManager.playerDeck = deck;
            
            // Load all created cards and add 3 copies each to deck
            string[] cardGuids = UnityEditor.AssetDatabase.FindAssets("t:Card", new[] { "Assets/Cards" });
            foreach (string guid in cardGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Card card = UnityEditor.AssetDatabase.LoadAssetAtPath<Card>(path);
                if (card != null)
                {
                    deck.startingCards.Add(new DeckEntry(card, 3));
                }
            }
            
            Debug.Log($"✓ Deck created with {deck.startingCards.Count * 3} cards");
        }
        
        // Create Card Grid
        if (gameManager.cardGrid == null)
        {
            GameObject gridObj = new GameObject("CardGrid");
            CardGrid grid = gridObj.AddComponent<CardGrid>();
            gameManager.cardGrid = grid;
            gridObj.transform.position = Vector3.zero;
        }
        
        // Create Hand Manager
        if (gameManager.handManager == null)
        {
            GameObject handObj = new GameObject("HandManager");
            HandManager handManager = handObj.AddComponent<HandManager>();
            gameManager.handManager = handManager;
        }
        
        // Create Shop Manager
        if (gameManager.shopManager == null)
        {
            GameObject shopObj = new GameObject("ShopManager");
            ShopManager shopManager = shopObj.AddComponent<ShopManager>();
            gameManager.shopManager = shopManager;
            
            // Add all cards to shop
            string[] cardGuids = UnityEditor.AssetDatabase.FindAssets("t:Card", new[] { "Assets/Cards" });
            foreach (string guid in cardGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Card card = UnityEditor.AssetDatabase.LoadAssetAtPath<Card>(path);
                if (card != null)
                {
                    shopManager.availableCards.Add(card);
                    
                    if (card.rarity <= 1) shopManager.tier1Cards.Add(card);
                    else if (card.rarity == 2) shopManager.tier2Cards.Add(card);
                    else shopManager.tier3Cards.Add(card);
                }
            }
        }
        
        // Connect UI Manager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            gameManager.uiManager = uiManager;
        }
        
        Debug.Log("✓ All game components connected");
    }
}