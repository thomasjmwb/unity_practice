using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public Transform gridParent;
    public GameObject cardSlotPrefab;
    public Vector2 spacing = new Vector2(1.5f, 1.5f);
    
    private Card[,] grid = new Card[3, 3];
    private CardSlot[,] gridSlots = new CardSlot[3, 3];
    private List<Card> placedCards = new List<Card>();
    
    public System.Action<Card, GridPosition> OnCardPlaced;
    public System.Action<Card, GridPosition> OnCardRemoved;
    
    void Start()
    {
        InitializeGrid();
    }
    
    void InitializeGrid()
    {
        // If no prefab is provided, create simple data-only slots
        // UI representation is handled separately by UI Toolkit
        if (cardSlotPrefab == null)
        {
            Debug.Log("CardGrid: No slot prefab provided, using data-only mode for UI Toolkit");
            // Initialize the grid data structure without visual slots
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    // Create minimal GameObject for data purposes
                    GameObject slotObj = new GameObject($"DataSlot_{x}_{y}");
                    slotObj.transform.SetParent(gridParent);
                    
                    // Add a minimal CardSlot component for data handling
                    CardSlot slot = slotObj.AddComponent<CardSlot>();
                    slot.Initialize(new GridPosition(x, y), this);
                    gridSlots[x, y] = slot;
                }
            }
            return;
        }
        
        // Original code for when prefab is available
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3 position = new Vector3(
                    (x - 1) * spacing.x,
                    (y - 1) * spacing.y,
                    0
                );
                
                GameObject slotObj = Instantiate(cardSlotPrefab, gridParent);
                slotObj.transform.localPosition = position;
                
                CardSlot slot = slotObj.GetComponent<CardSlot>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<CardSlot>();
                }
                
                slot.Initialize(new GridPosition(x, y), this);
                gridSlots[x, y] = slot;
            }
        }
    }
    
    public bool PlaceCard(Card card, GridPosition position)
    {
        if (!IsValidPosition(position) || grid[position.x, position.y] != null)
        {
            return false;
        }
        
        grid[position.x, position.y] = card;
        placedCards.Add(card);
        gridSlots[position.x, position.y].SetCard(card);
        
        OnCardPlaced?.Invoke(card, position);
        return true;
    }
    
    public bool RemoveCard(GridPosition position)
    {
        if (!IsValidPosition(position) || grid[position.x, position.y] == null)
        {
            return false;
        }
        
        Card removedCard = grid[position.x, position.y];
        grid[position.x, position.y] = null;
        placedCards.Remove(removedCard);
        gridSlots[position.x, position.y].ClearCard();
        
        OnCardRemoved?.Invoke(removedCard, position);
        return true;
    }
    
    public Card GetCard(GridPosition position)
    {
        if (!IsValidPosition(position))
            return null;
            
        return grid[position.x, position.y];
    }
    
    public bool IsValidPosition(GridPosition position)
    {
        return position.x >= 0 && position.x < 3 && position.y >= 0 && position.y < 3;
    }
    
    public bool IsSlotEmpty(GridPosition position)
    {
        return IsValidPosition(position) && grid[position.x, position.y] == null;
    }
    
    public List<Card> GetAllCards()
    {
        return new List<Card>(placedCards);
    }
    
    public Dictionary<CardFaction, int> GetFactionCounts()
    {
        Dictionary<CardFaction, int> factionCounts = new Dictionary<CardFaction, int>();
        
        foreach (Card card in placedCards)
        {
            if (factionCounts.ContainsKey(card.faction))
            {
                factionCounts[card.faction]++;
            }
            else
            {
                factionCounts[card.faction] = 1;
            }
        }
        
        return factionCounts;
    }
    
    public int CalculateTotalGold(List<GlobalModifier> globalModifiers = null)
    {
        if (globalModifiers == null)
            globalModifiers = new List<GlobalModifier>();
            
        int totalGold = 0;
        var factionCounts = GetFactionCounts();
        
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Card card = grid[x, y];
                if (card != null)
                {
                    GridPosition position = new GridPosition(x, y);
                    totalGold += card.CalculateTotalGold(position, placedCards, factionCounts, globalModifiers);
                }
            }
        }
        
        return totalGold;
    }
    
    public void ClearGrid()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (grid[x, y] != null)
                {
                    RemoveCard(new GridPosition(x, y));
                }
            }
        }
    }
}