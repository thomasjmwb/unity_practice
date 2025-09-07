using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DeckEntry
{
    public Card card;
    public int count;
    
    public DeckEntry(Card card, int count = 1)
    {
        this.card = card;
        this.count = count;
    }
}

public class Deck : MonoBehaviour
{
    [Header("Deck Settings")]
    public List<DeckEntry> startingCards = new List<DeckEntry>();
    public int maxDeckSize = 50;
    
    private List<Card> currentDeck = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    
    public System.Action OnDeckChanged;
    public System.Action OnDeckEmpty;
    
    public int CardsInDeck => currentDeck.Count;
    public int CardsInDiscard => discardPile.Count;
    public int TotalCards => CardsInDeck + CardsInDiscard;
    
    void Start()
    {
        InitializeDeck();
    }
    
    public void InitializeDeck()
    {
        currentDeck.Clear();
        discardPile.Clear();
        
        foreach (var entry in startingCards)
        {
            for (int i = 0; i < entry.count; i++)
            {
                currentDeck.Add(entry.card);
            }
        }
        
        ShuffleDeck();
        OnDeckChanged?.Invoke();
    }
    
    public void ShuffleDeck()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            Card temp = currentDeck[i];
            int randomIndex = Random.Range(i, currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;
        }
    }
    
    public Card DrawCard()
    {
        if (currentDeck.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                OnDeckEmpty?.Invoke();
                return null;
            }
            
            ReshuffleDiscardIntoDeck();
        }
        
        Card drawnCard = currentDeck[0];
        currentDeck.RemoveAt(0);
        OnDeckChanged?.Invoke();
        
        return drawnCard;
    }
    
    public List<Card> DrawCards(int count)
    {
        List<Card> drawnCards = new List<Card>();
        
        for (int i = 0; i < count; i++)
        {
            Card card = DrawCard();
            if (card != null)
            {
                drawnCards.Add(card);
            }
            else
            {
                break;
            }
        }
        
        return drawnCards;
    }
    
    public void AddCardToDeck(Card card)
    {
        if (TotalCards < maxDeckSize)
        {
            currentDeck.Add(card);
            OnDeckChanged?.Invoke();
        }
    }
    
    public void AddCardToDiscard(Card card)
    {
        discardPile.Add(card);
        OnDeckChanged?.Invoke();
    }
    
    public bool RemoveCardFromDeck(Card card)
    {
        bool removed = currentDeck.Remove(card);
        if (!removed)
        {
            removed = discardPile.Remove(card);
        }
        
        if (removed)
        {
            OnDeckChanged?.Invoke();
        }
        
        return removed;
    }
    
    public void ReshuffleDiscardIntoDeck()
    {
        currentDeck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
        OnDeckChanged?.Invoke();
    }
    
    public List<Card> GetAllCards()
    {
        List<Card> allCards = new List<Card>();
        allCards.AddRange(currentDeck);
        allCards.AddRange(discardPile);
        return allCards;
    }
    
    public Dictionary<Card, int> GetCardCounts()
    {
        Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
        
        foreach (Card card in GetAllCards())
        {
            if (cardCounts.ContainsKey(card))
            {
                cardCounts[card]++;
            }
            else
            {
                cardCounts[card] = 1;
            }
        }
        
        return cardCounts;
    }
}