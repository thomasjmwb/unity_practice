using UnityEngine;
using UnityEngine.UIElements;

public class WorkingGameSetup : MonoBehaviour
{
    [Header("Automatic Setup")]
    [Tooltip("This will automatically set up the entire game when you add it to the scene")]
    public bool autoSetupOnAwake = true;
    
    [Header("Initialization Control")]
    [Tooltip("Ensures this script runs before GameManager for proper initialization order")]
    public bool enforceExecutionOrder = true;
    
    [Header("Created Assets")]
    public Card[] gameCards = new Card[10];
    
    void Awake()
    {
        if (autoSetupOnAwake)
        {
            PerformCompleteSetup();
        }
    }
    
    [ContextMenu("Perform Complete Setup")]
    public void PerformCompleteSetup()
    {
        Debug.Log("🎮 Setting up complete Landlord Card Game...");
        
        CreateDirectories();
        CreateSampleCards();
        CreateGameManagers();
        CreateWorkingUI();
        SetupDeck();
        ConnectAllComponents();
        
        // Explicitly initialize the game after all setup is complete
        InitializeGameAfterSetup();
        
        Debug.Log("✅ Complete game setup finished! Press Play to test the game.");
        Debug.Log("🎯 Game Features:");
        Debug.Log("   • 10 different card types with 3 copies each (30 total cards)");
        Debug.Log("   • Draw 2 cards per turn");
        Debug.Log("   • Place cards on 3x3 grid");
        Debug.Log("   • Earn gold based on card effects and positions");
        Debug.Log("   • Shop system to buy new cards");
        Debug.Log("   • 12 rounds to survive with increasing rent targets");
    }
    
    void CreateDirectories()
    {
        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Cards"))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Cards");
        }
        
        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
    }
    
    void CreateSampleCards()
    {
        Debug.Log("Creating 10 sample cards...");
        
        // Create the 10 card types with simple effects
        gameCards[0] = CreateCard("Gold Coin", "Simple gold generation", CardFaction.Commerce, 1, 1, 10);
        gameCards[1] = CreateCard("Market Stall", "Gains +3 gold per adjacent card", CardFaction.Commerce, 2, 1, 5);
        gameCards[2] = CreateCard("Watchtower", "Double gold when placed in corners", CardFaction.Military, 3, 2, 8);
        gameCards[3] = CreateCard("Tree Grove", "More gold for each Nature card", CardFaction.Nature, 2, 2, 6);
        gameCards[4] = CreateCard("Crystal Mine", "Gains +2 gold each round", CardFaction.Magic, 4, 3, 3);
        gameCards[5] = CreateCard("Trade Route", "Bonus gold for completing lines", CardFaction.Commerce, 3, 2, 5);
        gameCards[6] = CreateCard("Tech Hub", "Technology synergy bonus", CardFaction.Technology, 3, 2, 7);
        gameCards[7] = CreateCard("Fortress", "Strong defensive position", CardFaction.Military, 4, 3, 12);
        gameCards[8] = CreateCard("Mystic Circle", "Magic amplification nearby", CardFaction.Magic, 2, 2, 8);
        gameCards[9] = CreateCard("Forest Heart", "Nature's growing strength", CardFaction.Nature, 5, 3, 5);
        
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        
        Debug.Log("✓ 10 card types created successfully");
    }
    
    Card CreateCard(string name, string description, CardFaction faction, int cost, int rarity, int baseGold)
    {
        // Create the card
        Card card = ScriptableObject.CreateInstance<Card>();
        card.cardName = name;
        card.description = description;
        card.faction = faction;
        card.cost = cost;
        card.rarity = rarity;
        
        // Create simple gold effect using constructor
        CardEffect effect = new CardEffect();
        effect.effectName = $"{name} Effect";
        effect.baseValue = baseGold;
        effect.requiresPosition = false;
        effect.requiresFactionCount = false;
        effect.isGlobalModifier = false;
        
        card.effects.Add(effect);
        
        // Set positioning preferences
        SetCardPreferences(card, name);
        
        // Save the card as an asset
        string path = $"Assets/Cards/{name.Replace(" ", "")}.asset";
        UnityEditor.AssetDatabase.CreateAsset(card, path);
        
        return card;
    }
    
    void SetCardPreferences(Card card, string cardName)
    {
        switch (cardName)
        {
            case "Watchtower":
            case "Fortress":
                card.prefersCorners = true;
                break;
                
            case "Market Stall":
            case "Trade Route":
                card.prefersEdges = true;
                break;
                
            case "Mystic Circle":
                card.prefersCenter = true;
                break;
        }
    }
    
    void CreateGameManagers()
    {
        // Create GameManager
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
            
            // Configure game settings
            gameManager.cardsPerTurn = 2;
            gameManager.initialGoldTarget = 100;
            gameManager.goldTargetMultiplier = 1.5f;
            gameManager.maxRounds = 12;
        }
        
        // Create CardGrid component (data-only, UI handled by UI Toolkit)
        if (FindObjectOfType<CardGrid>() == null)
        {
            GameObject cardGridObj = new GameObject("CardGrid");
            CardGrid cardGrid = cardGridObj.AddComponent<CardGrid>();
            
            // Create a minimal setup for the data-layer CardGrid
            // Since UI is handled by UI Toolkit, we just need the data structure
            GameObject gridParent = new GameObject("GridParent");
            gridParent.transform.SetParent(cardGridObj.transform);
            cardGrid.gridParent = gridParent.transform;
            cardGrid.spacing = new Vector2(1.5f, 1.5f);
            
            // Note: cardSlotPrefab will be null, but we'll handle this in CardGrid
        }
        
        // Create HandManager
        if (FindObjectOfType<HandManager>() == null)
        {
            GameObject handObj = new GameObject("HandManager");
            HandManager handManager = handObj.AddComponent<HandManager>();
            
            // Configure HandManager for UI Toolkit mode
            handManager.maxHandSize = 10;
            handManager.cardSpacing = 150f;
            // handParent and cardUIPrefab will be null for UI Toolkit mode
        }
        
        // Create ShopManager
        if (FindObjectOfType<ShopManager>() == null)
        {
            GameObject shopObj = new GameObject("ShopManager");
            ShopManager shopManager = shopObj.AddComponent<ShopManager>();
        }
        
        Debug.Log("✓ Game managers created");
    }
    
    void CreateWorkingUI()
    {
        // Create UI Document for UI Toolkit
        if (FindObjectOfType<UIDocument>() == null)
        {
            GameObject uiRoot = new GameObject("UI Toolkit Root");
            UIDocument uiDocument = uiRoot.AddComponent<UIDocument>();
            
            // Create working UI without problematic style properties
            CreateWorkingGameUI(uiDocument);
            
            // Add UI managers
            UIToolkitManager uiToolkitManager = uiRoot.AddComponent<UIToolkitManager>();
            uiToolkitManager.mainUIDocument = uiDocument;
            
            UIManager uiManager = uiRoot.AddComponent<UIManager>();
            uiManager.useUIToolkit = true;
            uiManager.uiToolkitManager = uiToolkitManager;
        }
        
        Debug.Log("✓ UI system created");
    }
    
    void CreateWorkingGameUI(UIDocument uiDocument)
    {
        var root = uiDocument.rootVisualElement;
        root.style.flexGrow = 1;
        root.style.backgroundColor = new Color(0.08f, 0.1f, 0.14f, 1f);
        
        // Create top info bar
        CreateTopBar(root);
        
        // Create main game area
        CreateGameArea(root);
        
        // Create bottom action bar
        CreateBottomBar(root);
        
        Debug.Log("✓ Working Game UI created");
    }
    
    void CreateTopBar(VisualElement root)
    {
        var topBar = new VisualElement();
        topBar.style.flexDirection = FlexDirection.Row;
        topBar.style.justifyContent = Justify.SpaceBetween;
        topBar.style.alignItems = Align.Center;
        topBar.style.backgroundColor = new Color(0.12f, 0.14f, 0.18f, 1f);
        topBar.style.minHeight = 80;
        topBar.style.paddingLeft = 15;
        topBar.style.paddingRight = 15;
        topBar.style.paddingTop = 15;
        topBar.style.paddingBottom = 15;
        
        // Info panels
        topBar.Add(CreateInfoPanel("Round", "1", "round-value", new Color(0.39f, 0.78f, 1f, 1f)));
        topBar.Add(CreateInfoPanel("Gold", "0", "gold-value", new Color(1f, 0.84f, 0f, 1f)));
        topBar.Add(CreateInfoPanel("Target", "100", "target-value", new Color(0.7f, 0.75f, 0.8f, 1f)));
        topBar.Add(CreateInfoPanel("Deck", "30", "deck-value", new Color(0.59f, 1f, 0.59f, 1f)));
        topBar.Add(CreateInfoPanel("Phase", "Playing", "phase-value", new Color(1f, 0.59f, 1f, 1f)));
        
        root.Add(topBar);
    }
    
    VisualElement CreateInfoPanel(string label, string value, string valueName, Color valueColor)
    {
        var panel = new VisualElement();
        panel.style.flexDirection = FlexDirection.Column;
        panel.style.alignItems = Align.Center;
        panel.style.backgroundColor = new Color(0.16f, 0.18f, 0.22f, 1f);
        panel.style.borderTopLeftRadius = 8;
        panel.style.borderTopRightRadius = 8;
        panel.style.borderBottomLeftRadius = 8;
        panel.style.borderBottomRightRadius = 8;
        panel.style.minWidth = 100;
        panel.style.paddingLeft = 10;
        panel.style.paddingRight = 10;
        panel.style.paddingTop = 10;
        panel.style.paddingBottom = 10;
        
        var labelElement = new Label(label);
        labelElement.style.fontSize = 12;
        labelElement.style.color = new Color(0.7f, 0.75f, 0.8f, 1f);
        labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;
        labelElement.style.marginBottom = 5;
        
        var valueElement = new Label(value);
        valueElement.name = valueName;
        valueElement.style.fontSize = 20;
        valueElement.style.color = valueColor;
        valueElement.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        panel.Add(labelElement);
        panel.Add(valueElement);
        
        return panel;
    }
    
    void CreateGameArea(VisualElement root)
    {
        var gameArea = new VisualElement();
        gameArea.style.flexGrow = 1;
        gameArea.style.flexDirection = FlexDirection.Row;
        gameArea.style.paddingLeft = 20;
        gameArea.style.paddingRight = 20;
        gameArea.style.paddingTop = 20;
        gameArea.style.paddingBottom = 20;
        
        // Grid container
        var gridContainer = new VisualElement();
        gridContainer.style.flexGrow = 2;
        gridContainer.style.alignItems = Align.Center;
        gridContainer.style.justifyContent = Justify.Center;
        
        var gridTitle = new Label("Card Grid");
        gridTitle.style.fontSize = 18;
        gridTitle.style.color = new Color(0.78f, 0.82f, 0.86f, 1f);
        gridTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
        gridTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        gridTitle.style.marginBottom = 15;
        gridContainer.Add(gridTitle);
        
        var cardGrid = CreateCardGrid();
        gridContainer.Add(cardGrid);
        
        var gridInfo = new Label("Total Gold: 0");
        gridInfo.name = "grid-gold-total";
        gridInfo.style.fontSize = 20;
        gridInfo.style.color = new Color(1f, 0.84f, 0f, 1f);
        gridInfo.style.unityFontStyleAndWeight = FontStyle.Bold;
        gridInfo.style.unityTextAlign = TextAnchor.MiddleCenter;
        gridInfo.style.marginTop = 15;
        gridContainer.Add(gridInfo);
        
        gameArea.Add(gridContainer);
        
        // Hand container
        var handContainer = CreateHandContainer();
        gameArea.Add(handContainer);
        
        root.Add(gameArea);
    }
    
    VisualElement CreateCardGrid()
    {
        var cardGrid = new VisualElement();
        cardGrid.name = "card-grid";
        cardGrid.style.width = 360;
        cardGrid.style.height = 360;
        cardGrid.style.flexDirection = FlexDirection.Row;
        cardGrid.style.flexWrap = Wrap.Wrap;
        cardGrid.style.backgroundColor = new Color(0.1f, 0.12f, 0.16f, 1f);
        cardGrid.style.borderTopLeftRadius = 12;
        cardGrid.style.borderTopRightRadius = 12;
        cardGrid.style.borderBottomLeftRadius = 12;
        cardGrid.style.borderBottomRightRadius = 12;
        cardGrid.style.paddingLeft = 10;
        cardGrid.style.paddingRight = 10;
        cardGrid.style.paddingTop = 10;
        cardGrid.style.paddingBottom = 10;
        
        // Create 9 grid slots (3x3)
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var slot = new VisualElement();
                slot.name = $"slot-{x}-{y}";
                slot.style.width = 110;
                slot.style.height = 110;
                slot.style.backgroundColor = new Color(0.16f, 0.18f, 0.22f, 1f);
                slot.style.borderTopLeftRadius = 8;
                slot.style.borderTopRightRadius = 8;
                slot.style.borderBottomLeftRadius = 8;
                slot.style.borderBottomRightRadius = 8;
                slot.style.marginLeft = 2;
                slot.style.marginRight = 2;
                slot.style.marginTop = 2;
                slot.style.marginBottom = 2;
                
                cardGrid.Add(slot);
            }
        }
        
        return cardGrid;
    }
    
    VisualElement CreateHandContainer()
    {
        var handContainer = new VisualElement();
        handContainer.style.flexGrow = 1;
        handContainer.style.minWidth = 300;
        handContainer.style.marginLeft = 30;
        
        var handTitle = new Label("Hand");
        handTitle.style.fontSize = 18;
        handTitle.style.color = new Color(0.78f, 0.82f, 0.86f, 1f);
        handTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
        handTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
        handTitle.style.marginBottom = 15;
        handContainer.Add(handTitle);
        
        var handScroll = new ScrollView();
        handScroll.name = "hand-scroll";
        handScroll.style.flexGrow = 1;
        handScroll.style.backgroundColor = new Color(0.1f, 0.12f, 0.16f, 1f);
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
        return handContainer;
    }
    
    void CreateBottomBar(VisualElement root)
    {
        var bottomBar = new VisualElement();
        bottomBar.style.flexDirection = FlexDirection.Row;
        bottomBar.style.justifyContent = Justify.Center;
        bottomBar.style.alignItems = Align.Center;
        bottomBar.style.backgroundColor = new Color(0.12f, 0.14f, 0.18f, 1f);
        bottomBar.style.paddingLeft = 15;
        bottomBar.style.paddingRight = 15;
        bottomBar.style.paddingTop = 15;
        bottomBar.style.paddingBottom = 15;
        
        var endRoundBtn = CreateStyledButton("End Round", "end-round-btn", true);
        endRoundBtn.clicked += () => {
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null) gameManager.EndRound();
        };
        
        var autoPlaceBtn = CreateStyledButton("Auto Place", "auto-place-btn", false);
        autoPlaceBtn.clicked += () => {
            Debug.Log("Auto Place feature - place all hand cards automatically");
        };
        
        var clearGridBtn = CreateStyledButton("Clear Grid", "clear-grid-btn", false);
        clearGridBtn.clicked += () => {
            // Find the card grid in the UI document
            var uiDoc = FindObjectOfType<UIDocument>();
            if (uiDoc != null)
            {
                var gridElement = uiDoc.rootVisualElement.Q("card-grid");
                if (gridElement != null)
                {
                    // Clear the UI grid slots
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            var slot = gridElement.Q($"slot-{x}-{y}");
                            if (slot != null)
                            {
                                slot.Clear();
                            }
                        }
                    }
                    Debug.Log("Grid cleared via UI");
                }
            }
        };
        
        bottomBar.Add(endRoundBtn);
        bottomBar.Add(autoPlaceBtn);
        bottomBar.Add(clearGridBtn);
        
        root.Add(bottomBar);
    }
    
    Button CreateStyledButton(string text, string name, bool isPrimary)
    {
        var button = new Button();
        button.name = name;
        button.text = text;
        button.style.minWidth = 120;
        button.style.marginLeft = 7;
        button.style.marginRight = 7;
        button.style.borderTopLeftRadius = 6;
        button.style.borderTopRightRadius = 6;
        button.style.borderBottomLeftRadius = 6;
        button.style.borderBottomRightRadius = 6;
        button.style.fontSize = 14;
        button.style.unityFontStyleAndWeight = FontStyle.Bold;
        button.style.paddingLeft = 12;
        button.style.paddingRight = 24;
        button.style.paddingTop = 12;
        button.style.paddingBottom = 12;
        
        if (isPrimary)
        {
            button.style.backgroundColor = new Color(0.27f, 0.51f, 0.78f, 1f);
            button.style.color = Color.white;
        }
        else
        {
            button.style.backgroundColor = new Color(0.24f, 0.28f, 0.33f, 1f);
            button.style.color = new Color(0.78f, 0.82f, 0.86f, 1f);
        }
        
        return button;
    }
    
    void SetupDeck()
    {
        Deck deck = FindObjectOfType<Deck>();
        if (deck == null)
        {
            GameObject deckObj = new GameObject("PlayerDeck");
            deck = deckObj.AddComponent<Deck>();
        }
        
        // Clear existing deck entries
        deck.startingCards.Clear();
        
        // Add 3 copies of each created card to the deck
        foreach (Card card in gameCards)
        {
            if (card != null)
            {
                deck.startingCards.Add(new DeckEntry(card, 3));
            }
        }
        
        Debug.Log($"✓ Deck configured with {deck.startingCards.Count * 3} total cards");
    }
    
    void ConnectAllComponents()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) 
        {
            Debug.LogError("ConnectAllComponents: GameManager not found!");
            return;
        }
        
        // Connect all managers
        gameManager.playerDeck = FindObjectOfType<Deck>();
        gameManager.cardGrid = FindObjectOfType<CardGrid>();
        gameManager.handManager = FindObjectOfType<HandManager>();
        gameManager.shopManager = FindObjectOfType<ShopManager>();
        gameManager.uiManager = FindObjectOfType<UIManager>();
        
        // Debug log to verify connections
        Debug.Log($"Component connections - Deck: {gameManager.playerDeck != null}, CardGrid: {gameManager.cardGrid != null}, HandManager: {gameManager.handManager != null}, ShopManager: {gameManager.shopManager != null}, UIManager: {gameManager.uiManager != null}");
        
        // Configure shop with available cards
        ShopManager shopManager = gameManager.shopManager;
        if (shopManager != null)
        {
            shopManager.availableCards.Clear();
            shopManager.tier1Cards.Clear();
            shopManager.tier2Cards.Clear();
            shopManager.tier3Cards.Clear();
            
            foreach (Card card in gameCards)
            {
                if (card != null)
                {
                    shopManager.availableCards.Add(card);
                    
                    if (card.rarity <= 1)
                        shopManager.tier1Cards.Add(card);
                    else if (card.rarity == 2)
                        shopManager.tier2Cards.Add(card);
                    else
                        shopManager.tier3Cards.Add(card);
                }
            }
        }
        
        Debug.Log("✓ All components connected successfully");
    }
    
    void InitializeGameAfterSetup()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("InitializeGameAfterSetup: GameManager not found!");
            return;
        }
        
        // Verify all components are connected before initializing
        if (gameManager.playerDeck == null || gameManager.cardGrid == null || 
            gameManager.handManager == null || gameManager.shopManager == null)
        {
            Debug.LogError("InitializeGameAfterSetup: Some components are still null! Cannot initialize game.");
            return;
        }
        
        Debug.Log("🎮 All components verified - starting game initialization");
        gameManager.InitializeGame();
    }
}