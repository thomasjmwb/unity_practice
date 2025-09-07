using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI System Choice")]
    public bool useUIToolkit = true;
    public UIToolkitManager uiToolkitManager;
    
    [Header("Legacy UI (if not using UI Toolkit)")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI deckCountText;
    public Slider goldProgressSlider;
    
    [Header("Game State UI")]
    public GameObject playingUI;
    public GameObject shoppingUI;
    public GameObject gameOverUI;
    public GameObject victoryUI;
    
    [Header("Buttons")]
    public Button endRoundButton;
    public Button closeShopButton;
    public Button restartButton;
    public Button quitButton;
    
    [Header("Shop UI")]
    public TextMeshProUGUI shopGoldText;
    public Button refreshShopButton;
    public int refreshCost = 10;
    
    void Start()
    {
        if (useUIToolkit && uiToolkitManager != null)
        {
            SetupUIToolkitListeners();
        }
        else
        {
            SetupButtonListeners();
            UpdateGameStateUI(GameState.MainMenu);
        }
    }
    
    void SetupUIToolkitListeners()
    {
        if (uiToolkitManager == null) return;
        
        uiToolkitManager.OnEndRound += () => GameManager.Instance?.EndRound();
        uiToolkitManager.OnAutoPlace += AutoPlaceCards;
        uiToolkitManager.OnClearGrid += () => GameManager.Instance?.cardGrid?.ClearGrid();
        uiToolkitManager.OnCloseShop += () => FindObjectOfType<ShopManager>()?.CloseShop();
        uiToolkitManager.OnRefreshShop += () => FindObjectOfType<ShopManager>()?.RefreshShop(refreshCost);
        uiToolkitManager.OnContinue += () => FindObjectOfType<ShopManager>()?.CloseShop();
        uiToolkitManager.OnRestart += () => GameManager.Instance?.RestartGame();
        uiToolkitManager.OnQuit += () => GameManager.Instance?.QuitGame();
    }
    
    void AutoPlaceCards()
    {
        // Auto-place logic for cards in hand
        HandManager handManager = FindObjectOfType<HandManager>();
        CardGrid cardGrid = FindObjectOfType<CardGrid>();
        
        if (handManager == null || cardGrid == null) return;
        
        var cardsInHand = handManager.GetCardsInHand();
        
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    GridPosition pos = new GridPosition(x, y);
                    if (cardGrid.IsSlotEmpty(pos))
                    {
                        if (cardGrid.PlaceCard(cardsInHand[i], pos))
                        {
                            handManager.RemoveCardFromHand(cardsInHand[i]);
                            goto NextCard;
                        }
                    }
                }
            }
            NextCard:;
        }
    }
    
    void SetupButtonListeners()
    {
        if (endRoundButton != null)
            endRoundButton.onClick.AddListener(() => GameManager.Instance?.EndRound());
            
        if (closeShopButton != null)
            closeShopButton.onClick.AddListener(() => FindObjectOfType<ShopManager>()?.CloseShop());
            
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance?.RestartGame());
            
        if (quitButton != null)
            quitButton.onClick.AddListener(() => GameManager.Instance?.QuitGame());
            
        if (refreshShopButton != null)
            refreshShopButton.onClick.AddListener(() => FindObjectOfType<ShopManager>()?.RefreshShop(refreshCost));
    }
    
    public void UpdateGameInfo(int round, int gold, int target, GameState state)
    {
        if (useUIToolkit && uiToolkitManager != null)
        {
            uiToolkitManager.UpdateGameInfo(round, gold, target, state);
            return;
        }
        
        // Legacy UI updates
        if (roundText != null)
            roundText.text = $"Round: {round}";
            
        if (goldText != null)
            goldText.text = $"Gold: {gold}";
            
        if (targetText != null)
            targetText.text = $"Target: {target}";
            
        if (goldProgressSlider != null)
        {
            goldProgressSlider.maxValue = target;
            goldProgressSlider.value = gold;
        }
        
        UpdateDeckCount();
        UpdateGameStateUI(state);
        UpdateShopUI(gold);
    }
    
    void UpdateDeckCount()
    {
        if (deckCountText != null)
        {
            Deck playerDeck = FindObjectOfType<Deck>();
            if (playerDeck != null)
            {
                deckCountText.text = $"Deck: {playerDeck.CardsInDeck}";
            }
        }
    }
    
    void UpdateGameStateUI(GameState state)
    {
        if (playingUI != null)
            playingUI.SetActive(state == GameState.Playing);
            
        if (shoppingUI != null)
            shoppingUI.SetActive(state == GameState.Shopping);
            
        if (gameOverUI != null)
            gameOverUI.SetActive(state == GameState.GameOver);
            
        if (victoryUI != null)
            victoryUI.SetActive(state == GameState.Victory);
    }
    
    void UpdateShopUI(int gold)
    {
        if (shopGoldText != null)
            shopGoldText.text = $"Gold: {gold}";
            
        if (refreshShopButton != null)
        {
            refreshShopButton.interactable = gold >= refreshCost;
            
            TextMeshProUGUI refreshText = refreshShopButton.GetComponentInChildren<TextMeshProUGUI>();
            if (refreshText != null)
            {
                refreshText.text = $"Refresh ({refreshCost}g)";
            }
        }
    }
    
    public void ShowMessage(string message, float duration = 3f)
    {
        Debug.Log($"UI Message: {message}");
    }
    
    public void UpdateCardPreview(Card card)
    {
        if (card == null) return;
        
        Debug.Log($"Previewing card: {card.cardName}");
    }
    
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateGameStateUI;
        }
    }
    
    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateGameStateUI;
        }
    }
}