using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class UIToolkitManager : MonoBehaviour
{
    [Header("UI Documents")]
    public UIDocument mainUIDocument;
    public VisualTreeAsset cardTemplate;
    
    [Header("UXML Assets")]
    public VisualTreeAsset mainGameUI;
    public VisualTreeAsset cardUI;
    
    [Header("Style Sheets")]
    public StyleSheet gameStyles;
    public StyleSheet cardStyles;
    
    private VisualElement root;
    
    // Main UI Elements
    private Label roundValue;
    private Label goldValue;
    private Label targetValue;
    private Label deckValue;
    private Label phaseValue;
    private ProgressBar goldProgress;
    private Label gridGoldTotal;
    
    // Game Area Elements
    private VisualElement cardGrid;
    private VisualElement handCards;
    
    // Button Elements
    private Button endRoundBtn;
    private Button autoPlaceBtn;
    private Button clearGridBtn;
    
    // Shop Elements
    private VisualElement shopOverlay;
    private Label shopGoldValue;
    private VisualElement shopItems;
    private Button closeShopBtn;
    private Button refreshShopBtn;
    private Button continueBtn;
    
    // Game Over/Victory Elements
    private VisualElement gameOverOverlay;
    private VisualElement victoryOverlay;
    private Button restartBtn;
    private Button quitBtn;
    private Button victoryRestartBtn;
    private Button victoryQuitBtn;
    
    // Grid Management
    private VisualElement[,] gridSlots = new VisualElement[3, 3];
    private List<VisualElement> handCardElements = new List<VisualElement>();
    
    public System.Action OnEndRound;
    public System.Action OnAutoPlace;
    public System.Action OnClearGrid;
    public System.Action OnCloseShop;
    public System.Action OnRefreshShop;
    public System.Action OnContinue;
    public System.Action OnRestart;
    public System.Action OnQuit;
    
    void Awake()
    {
        if (mainUIDocument == null)
            mainUIDocument = GetComponent<UIDocument>();
    }
    
    void Start()
    {
        InitializeUI();
        SetupEventHandlers();
        InitializeGrid();
        
        // Add some sample cards to make UI visible for testing
        AddSampleCardsForTesting();
    }
    
    void AddSampleCardsForTesting()
    {
        Debug.Log("Adding sample cards for testing...");
        
        // Create some dummy cards to show the UI working
        if (handCards != null)
        {
            Debug.Log("Hand cards container found, adding sample cards");
            for (int i = 0; i < 3; i++)
            {
                VisualElement sampleCard = CreateSampleCard($"Sample Card {i + 1}", i + 1);
                handCards.Add(sampleCard);
                Debug.Log($"Added sample card {i + 1}");
            }
        }
        else
        {
            Debug.LogError("Hand cards container not found! Check UXML structure.");
        }
        
        // Also place one card on the grid as an example
        if (gridSlots[1, 1] != null)
        {
            Debug.Log("Grid center slot found, adding sample card");
            VisualElement gridCard = CreateSampleCard("Grid Card", 2);
            gridCard.RemoveFromClassList("hand-card");
            gridCard.AddToClassList("grid-card");
            gridSlots[1, 1].Add(gridCard);
            gridSlots[1, 1].AddToClassList("occupied");
        }
        else
        {
            Debug.LogError("Grid slot [1,1] not found! Check grid initialization.");
        }
        
        Debug.Log("Sample card addition complete");
    }
    
    VisualElement CreateSampleCard(string cardName, int cost)
    {
        VisualElement cardElement = new VisualElement();
        cardElement.AddToClassList("hand-card");
        
        // Header container
        VisualElement header = new VisualElement();
        header.AddToClassList("card-header");
        
        // Card name
        Label nameLabel = new Label(cardName);
        nameLabel.AddToClassList("card-name");
        
        // Cost
        Label costLabel = new Label(cost.ToString());
        costLabel.AddToClassList("card-cost");
        
        header.Add(nameLabel);
        header.Add(costLabel);
        cardElement.Add(header);
        
        // Art container
        VisualElement artContainer = new VisualElement();
        artContainer.AddToClassList("card-art-container");
        VisualElement art = new VisualElement();
        art.AddToClassList("card-art");
        art.AddToClassList("placeholder");
        art.style.backgroundColor = new Color(0.3f, 0.4f, 0.6f, 1f); // Blue placeholder
        art.style.height = 80;
        artContainer.Add(art);
        cardElement.Add(artContainer);
        
        // Description
        Label descLabel = new Label("This is a sample card for testing the UI.");
        descLabel.AddToClassList("card-description");
        cardElement.Add(descLabel);
        
        // Footer with faction
        VisualElement footer = new VisualElement();
        footer.AddToClassList("card-stats");
        
        Label factionLabel = new Label("Commerce");
        factionLabel.AddToClassList("card-faction");
        factionLabel.AddToClassList("faction-commerce");
        footer.Add(factionLabel);
        
        cardElement.Add(footer);
        
        return cardElement;
    }
    
    void InitializeUI()
    {
        if (mainUIDocument == null)
        {
            Debug.LogError("UI Document is not assigned!");
            return;
        }
        
        root = mainUIDocument.rootVisualElement;
        
        if (root == null)
        {
            Debug.LogError("Root visual element is null! Check if UXML is properly loaded.");
            return;
        }
        
        Debug.Log($"UI Root found with {root.childCount} children");
        
        // Apply style sheets
        if (gameStyles != null)
        {
            root.styleSheets.Add(gameStyles);
            Debug.Log("Game styles applied to root");
        }
        else
        {
            Debug.LogWarning("Game styles are null!");
        }
        
        if (cardStyles != null)
        {
            root.styleSheets.Add(cardStyles);
            Debug.Log("Card styles applied to root");
        }
        else
        {
            Debug.LogWarning("Card styles are null!");
        }
        
        // Get UI element references
        GetUIReferences();
    }
    
    void GetUIReferences()
    {
        Debug.Log("Getting UI element references...");
        
        // Top bar elements
        roundValue = root.Q<Label>("round-value");
        goldValue = root.Q<Label>("gold-value");
        targetValue = root.Q<Label>("target-value");
        deckValue = root.Q<Label>("deck-value");
        phaseValue = root.Q<Label>("phase-value");
        goldProgress = root.Q<ProgressBar>("gold-progress");
        
        // Game area elements
        cardGrid = root.Q<VisualElement>("card-grid");
        handCards = root.Q<VisualElement>("hand-cards");
        gridGoldTotal = root.Q<Label>("grid-gold-total");
        
        Debug.Log($"Card grid found: {cardGrid != null}");
        Debug.Log($"Hand cards found: {handCards != null}");
        
        // Button elements
        endRoundBtn = root.Q<Button>("end-round-btn");
        autoPlaceBtn = root.Q<Button>("auto-place-btn");
        clearGridBtn = root.Q<Button>("clear-grid-btn");
        
        // Shop elements
        shopOverlay = root.Q<VisualElement>("shop-overlay");
        shopGoldValue = root.Q<Label>("shop-gold-value");
        shopItems = root.Q<VisualElement>("shop-items");
        closeShopBtn = root.Q<Button>("close-shop-btn");
        refreshShopBtn = root.Q<Button>("refresh-shop-btn");
        continueBtn = root.Q<Button>("continue-btn");
        
        // Game over/victory elements
        gameOverOverlay = root.Q<VisualElement>("game-over-overlay");
        victoryOverlay = root.Q<VisualElement>("victory-overlay");
        restartBtn = root.Q<Button>("restart-btn");
        quitBtn = root.Q<Button>("quit-btn");
        victoryRestartBtn = root.Q<Button>("victory-restart-btn");
        victoryQuitBtn = root.Q<Button>("victory-quit-btn");
        
        Debug.Log("UI references gathered");
    }
    
    void SetupEventHandlers()
    {
        endRoundBtn?.RegisterCallback<ClickEvent>(evt => OnEndRound?.Invoke());
        autoPlaceBtn?.RegisterCallback<ClickEvent>(evt => OnAutoPlace?.Invoke());
        clearGridBtn?.RegisterCallback<ClickEvent>(evt => OnClearGrid?.Invoke());
        
        closeShopBtn?.RegisterCallback<ClickEvent>(evt => OnCloseShop?.Invoke());
        refreshShopBtn?.RegisterCallback<ClickEvent>(evt => OnRefreshShop?.Invoke());
        continueBtn?.RegisterCallback<ClickEvent>(evt => OnContinue?.Invoke());
        
        restartBtn?.RegisterCallback<ClickEvent>(evt => OnRestart?.Invoke());
        quitBtn?.RegisterCallback<ClickEvent>(evt => OnQuit?.Invoke());
        victoryRestartBtn?.RegisterCallback<ClickEvent>(evt => OnRestart?.Invoke());
        victoryQuitBtn?.RegisterCallback<ClickEvent>(evt => OnQuit?.Invoke());
    }
    
    void InitializeGrid()
    {
        if (cardGrid == null) return;
        
        cardGrid.Clear();
        
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList("grid-slot");
                slot.name = $"slot-{x}-{y}";
                
                // Store reference
                gridSlots[x, y] = slot;
                
                // Add drop handling
                SetupDropHandling(slot, x, y);
                
                cardGrid.Add(slot);
            }
        }
    }
    
    void SetupDropHandling(VisualElement slot, int x, int y)
    {
        slot.RegisterCallback<DragEnterEvent>(evt => {
            slot.AddToClassList("drop-target");
        });
        
        slot.RegisterCallback<DragLeaveEvent>(evt => {
            slot.RemoveFromClassList("drop-target");
        });
        
        slot.RegisterCallback<DragUpdatedEvent>(evt => {
            // Handle drag visual feedback here
            // DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        });
        
        slot.RegisterCallback<DragPerformEvent>(evt => {
            slot.RemoveFromClassList("drop-target");
            // Handle card placement logic here
            HandleCardDrop(x, y, evt);
        });
    }
    
    void HandleCardDrop(int x, int y, DragPerformEvent evt)
    {
        // This would integrate with your card placement logic
        GridPosition position = new GridPosition(x, y);
        
        // Get the card being dragged (this would come from your drag system)
        // For now, we'll just show a placeholder
        PlaceCardOnGrid(null, position);
    }
    
    public void UpdateGameInfo(int round, int gold, int target, GameState state)
    {
        if (roundValue != null)
            roundValue.text = round.ToString();
            
        if (goldValue != null)
            goldValue.text = gold.ToString();
            
        if (targetValue != null)
            targetValue.text = $"/ {target}";
            
        if (goldProgress != null)
        {
            goldProgress.lowValue = 0;
            goldProgress.highValue = target;
            goldProgress.value = gold;
        }
        
        if (phaseValue != null)
            phaseValue.text = state.ToString();
            
        UpdateGameStateVisibility(state);
    }
    
    public void UpdateDeckCount(int count)
    {
        if (deckValue != null)
            deckValue.text = count.ToString();
    }
    
    public void UpdateGridGoldTotal(int total)
    {
        if (gridGoldTotal != null)
            gridGoldTotal.text = $"Total Gold: {total}";
    }
    
    void UpdateGameStateVisibility(GameState state)
    {
        // Hide all overlays first
        shopOverlay?.RemoveFromClassList("visible");
        shopOverlay?.AddToClassList("hidden");
        
        gameOverOverlay?.RemoveFromClassList("visible");
        gameOverOverlay?.AddToClassList("hidden");
        
        victoryOverlay?.RemoveFromClassList("visible");
        victoryOverlay?.AddToClassList("hidden");
        
        // Show appropriate overlay
        switch (state)
        {
            case GameState.Shopping:
                shopOverlay?.RemoveFromClassList("hidden");
                shopOverlay?.AddToClassList("visible");
                break;
                
            case GameState.GameOver:
                gameOverOverlay?.RemoveFromClassList("hidden");
                gameOverOverlay?.AddToClassList("visible");
                break;
                
            case GameState.Victory:
                victoryOverlay?.RemoveFromClassList("hidden");
                victoryOverlay?.AddToClassList("visible");
                break;
        }
    }
    
    public void AddCardToHand(Card card)
    {
        if (handCards == null || card == null) return;
        
        VisualElement cardElement = CreateCardElement(card, false);
        handCardElements.Add(cardElement);
        handCards.Add(cardElement);
        
        SetupCardDragging(cardElement, card);
    }
    
    public void RemoveCardFromHand(Card card)
    {
        // Find and remove the card element
        for (int i = handCardElements.Count - 1; i >= 0; i--)
        {
            var cardElement = handCardElements[i];
            if (cardElement.userData as Card == card)
            {
                handCards.Remove(cardElement);
                handCardElements.RemoveAt(i);
                break;
            }
        }
    }
    
    public void ClearHand()
    {
        handCards?.Clear();
        handCardElements.Clear();
    }
    
    public void PlaceCardOnGrid(Card card, GridPosition position)
    {
        if (gridSlots[position.x, position.y] == null) return;
        
        var slot = gridSlots[position.x, position.y];
        slot.Clear();
        
        if (card != null)
        {
            VisualElement cardElement = CreateCardElement(card, true);
            slot.Add(cardElement);
            slot.AddToClassList("occupied");
        }
        else
        {
            slot.RemoveFromClassList("occupied");
        }
    }
    
    public void ClearGrid()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                var slot = gridSlots[x, y];
                slot?.Clear();
                slot?.RemoveFromClassList("occupied");
            }
        }
    }
    
    VisualElement CreateCardElement(Card card, bool isGridVersion)
    {
        VisualElement cardElement = new VisualElement();
        cardElement.AddToClassList(isGridVersion ? "grid-card" : "hand-card");
        cardElement.userData = card;
        
        // Card name
        Label nameLabel = new Label(card.cardName);
        nameLabel.AddToClassList("card-name");
        cardElement.Add(nameLabel);
        
        if (!isGridVersion)
        {
            // Cost
            Label costLabel = new Label(card.cost.ToString());
            costLabel.AddToClassList("card-cost");
            
            // Header container
            VisualElement header = new VisualElement();
            header.AddToClassList("card-header");
            header.Add(nameLabel);
            header.Add(costLabel);
            cardElement.Clear();
            cardElement.Add(header);
            
            // Art container
            VisualElement artContainer = new VisualElement();
            artContainer.AddToClassList("card-art-container");
            VisualElement art = new VisualElement();
            art.AddToClassList("card-art");
            if (card.cardArt != null)
            {
                art.style.backgroundImage = new StyleBackground(card.cardArt);
            }
            else
            {
                art.AddToClassList("placeholder");
            }
            artContainer.Add(art);
            cardElement.Add(artContainer);
            
            // Description
            Label descLabel = new Label(card.description);
            descLabel.AddToClassList("card-description");
            cardElement.Add(descLabel);
        }
        
        // Footer with faction and effects
        VisualElement footer = new VisualElement();
        footer.AddToClassList("card-stats");
        
        Label factionLabel = new Label(card.faction.ToString());
        factionLabel.AddToClassList("card-faction");
        factionLabel.AddToClassList($"faction-{card.faction.ToString().ToLower()}");
        footer.Add(factionLabel);
        
        cardElement.Add(footer);
        
        return cardElement;
    }
    
    void SetupCardDragging(VisualElement cardElement, Card card)
    {
        cardElement.RegisterCallback<MouseDownEvent>(evt => {
            if (evt.button == 0) // Left mouse button
            {
                cardElement.AddToClassList("dragging");
                // Start drag operation - this would integrate with Unity's drag system
            }
        });
        
        cardElement.RegisterCallback<MouseUpEvent>(evt => {
            cardElement.RemoveFromClassList("dragging");
        });
    }
    
    public void UpdateShopGold(int gold)
    {
        if (shopGoldValue != null)
            shopGoldValue.text = $"Gold: {gold}";
    }
    
    public void AddShopItem(Card card, int price, bool canAfford)
    {
        if (shopItems == null || card == null) return;
        
        VisualElement shopCard = CreateCardElement(card, false);
        shopCard.RemoveFromClassList("hand-card");
        shopCard.AddToClassList("shop-card");
        
        if (!canAfford)
        {
            shopCard.AddToClassList("expensive");
        }
        else
        {
            shopCard.AddToClassList("affordable");
        }
        
        // Add price and buy button
        VisualElement priceContainer = new VisualElement();
        priceContainer.AddToClassList("shop-price-container");
        
        Label priceLabel = new Label($"{price}g");
        priceLabel.AddToClassList("shop-price");
        
        Button buyButton = new Button(() => {
            // Handle purchase
            HandleShopPurchase(card, price);
        });
        buyButton.text = "Buy";
        buyButton.AddToClassList("shop-buy-button");
        buyButton.SetEnabled(canAfford);
        
        priceContainer.Add(priceLabel);
        priceContainer.Add(buyButton);
        shopCard.Add(priceContainer);
        
        shopItems.Add(shopCard);
    }
    
    void HandleShopPurchase(Card card, int price)
    {
        // This would integrate with your shop manager
        Debug.Log($"Attempting to purchase {card.cardName} for {price} gold");
    }
    
    public void ClearShop()
    {
        shopItems?.Clear();
    }
    
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            OnEndRound += () => GameManager.Instance.EndRound();
            OnClearGrid += () => GameManager.Instance.cardGrid?.ClearGrid();
            OnRestart += () => GameManager.Instance.RestartGame();
            OnQuit += () => GameManager.Instance.QuitGame();
        }
    }
}