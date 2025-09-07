using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum GameState
{
    MainMenu,
    Playing,
    Shopping,
    GameOver,
    Victory
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int cardsPerTurn = 2;
    public int initialGoldTarget = 100;
    public float goldTargetMultiplier = 1.5f;
    public int maxRounds = 12;
    
    [Header("Components")]
    public Deck playerDeck;
    public CardGrid cardGrid;
    public HandManager handManager;
    public ShopManager shopManager;
    public UIManager uiManager;
    
    [Header("Game State")]
    public int currentRound = 1;
    public int currentGold = 0;
    public int currentGoldTarget;
    public GameState currentState = GameState.MainMenu;
    
    private List<GlobalModifier> globalModifiers = new List<GlobalModifier>();
    private bool isInitialized = false;
    
    public System.Action<GameState> OnGameStateChanged;
    public System.Action<int> OnGoldChanged;
    public System.Action<int> OnRoundChanged;
    public System.Action OnGameWon;
    public System.Action OnGameLost;
    
    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Don't auto-initialize - wait for explicit initialization from setup
        Debug.Log("GameManager: Waiting for explicit initialization from WorkingGameSetup");
    }
    
    public void InitializeGame()
    {
        if (isInitialized)
        {
            Debug.LogWarning("GameManager: Already initialized, skipping duplicate initialization");
            return;
        }
        
        // Verify all required components are connected
        if (playerDeck == null || cardGrid == null || handManager == null)
        {
            Debug.LogError("GameManager: Cannot initialize - missing required components!");
            Debug.LogError($"playerDeck: {playerDeck != null}, cardGrid: {cardGrid != null}, handManager: {handManager != null}");
            return;
        }
        
        Debug.Log("🎮 GameManager: Starting game initialization");
        
        currentRound = 1;
        currentGold = 0;
        currentGoldTarget = initialGoldTarget;
        globalModifiers.Clear();
        
        // Initialize the deck first
        if (playerDeck != null)
        {
            playerDeck.InitializeDeck();
        }
        
        ChangeGameState(GameState.Playing);
        isInitialized = true;
        
        Debug.Log("✅ GameManager: Initialization complete, starting first round");
        StartNewRound();
    }
    
    public void StartNewRound()
    {
        Debug.Log($"Starting Round {currentRound} - Target: {currentGoldTarget} gold");
        
        // Safety checks with helpful error messages
        if (cardGrid == null)
        {
            Debug.LogError("GameManager: cardGrid is null! Make sure WorkingGameSetup has run and connected all components.");
            return;
        }
        
        if (handManager == null)
        {
            Debug.LogError("GameManager: handManager is null! Make sure WorkingGameSetup has run and connected all components.");
            return;
        }
        
        if (playerDeck == null)
        {
            Debug.LogError("GameManager: playerDeck is null! Make sure WorkingGameSetup has run and connected all components.");
            return;
        }
        
        cardGrid.ClearGrid();
        handManager.ClearHand();
        
        List<Card> drawnCards = playerDeck.DrawCards(cardsPerTurn);
        handManager.AddCardsToHand(drawnCards);
        
        OnRoundChanged?.Invoke(currentRound);
        UpdateUI();
    }
    
    public void EndRound()
    {
        int earnedGold = cardGrid.CalculateTotalGold(globalModifiers);
        AddGold(earnedGold);
        
        Debug.Log($"Round {currentRound} - Earned: {earnedGold} gold, Total: {currentGold} gold");
        
        if (currentGold >= currentGoldTarget)
        {
            if (currentRound >= maxRounds)
            {
                WinGame();
            }
            else
            {
                AdvanceToShopping();
            }
        }
        else
        {
            LoseGame();
        }
    }
    
    void AdvanceToShopping()
    {
        currentGold -= currentGoldTarget;
        currentRound++;
        currentGoldTarget = Mathf.RoundToInt(currentGoldTarget * goldTargetMultiplier);
        
        ChangeGameState(GameState.Shopping);
        shopManager.OpenShop(currentGold);
    }
    
    public void FinishShopping(int remainingGold)
    {
        currentGold = remainingGold;
        ChangeGameState(GameState.Playing);
        StartNewRound();
    }
    
    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
        UpdateUI();
    }
    
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void AddGlobalModifier(GlobalModifier modifier)
    {
        globalModifiers.Add(modifier);
    }
    
    public void RemoveGlobalModifier(GlobalModifier modifier)
    {
        globalModifiers.Remove(modifier);
    }
    
    void ChangeGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
        UpdateUI();
    }
    
    void WinGame()
    {
        Debug.Log("Victory! You defeated capitalism!");
        ChangeGameState(GameState.Victory);
        OnGameWon?.Invoke();
    }
    
    void LoseGame()
    {
        Debug.Log("Game Over! You couldn't pay the rent!");
        ChangeGameState(GameState.GameOver);
        OnGameLost?.Invoke();
    }
    
    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateGameInfo(currentRound, currentGold, currentGoldTarget, currentState);
        }
    }
    
    public void RestartGame()
    {
        isInitialized = false;
        if (playerDeck != null)
        {
            playerDeck.InitializeDeck();
        }
        InitializeGame();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}