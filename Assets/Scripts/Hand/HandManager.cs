using UnityEngine;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform handParent;
    public GameObject cardUIPrefab;
    public int maxHandSize = 10;
    public float cardSpacing = 150f;
    
    private List<Card> cardsInHand = new List<Card>();
    private List<CardUI> cardUIs = new List<CardUI>();
    
    public System.Action<Card> OnCardAddedToHand;
    public System.Action<Card> OnCardRemovedFromHand;
    public System.Action OnHandChanged;
    
    public int HandSize => cardsInHand.Count;
    public bool IsHandFull => cardsInHand.Count >= maxHandSize;
    
    void Start()
    {
        if (handParent == null)
            handParent = transform;
    }
    
    public void AddCardToHand(Card card)
    {
        if (IsHandFull || card == null) return;
        
        cardsInHand.Add(card);
        CreateCardUI(card);
        
        OnCardAddedToHand?.Invoke(card);
        OnHandChanged?.Invoke();
        UpdateHandLayout();
    }
    
    public void AddCardsToHand(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            AddCardToHand(card);
        }
    }
    
    public bool RemoveCardFromHand(Card card)
    {
        int index = cardsInHand.IndexOf(card);
        if (index == -1) return false;
        
        cardsInHand.RemoveAt(index);
        
        if (index < cardUIs.Count)
        {
            Destroy(cardUIs[index].gameObject);
            cardUIs.RemoveAt(index);
        }
        
        OnCardRemovedFromHand?.Invoke(card);
        OnHandChanged?.Invoke();
        UpdateHandLayout();
        
        return true;
    }
    
    public void ClearHand()
    {
        foreach (CardUI cardUI in cardUIs)
        {
            if (cardUI != null)
            {
                Destroy(cardUI.gameObject);
            }
        }
        
        cardsInHand.Clear();
        cardUIs.Clear();
        OnHandChanged?.Invoke();
    }
    
    void CreateCardUI(Card card)
    {
        GameObject cardObj = Instantiate(cardUIPrefab, handParent);
        CardUI cardUI = cardObj.GetComponent<CardUI>();
        
        if (cardUI == null)
        {
            cardUI = cardObj.AddComponent<CardUI>();
        }
        
        cardUI.Initialize(card, this);
        cardUIs.Add(cardUI);
    }
    
    void UpdateHandLayout()
    {
        int cardCount = cardUIs.Count;
        if (cardCount == 0) return;
        
        float totalWidth = (cardCount - 1) * cardSpacing;
        float startX = -totalWidth / 2f;
        
        for (int i = 0; i < cardUIs.Count; i++)
        {
            if (cardUIs[i] != null)
            {
                Vector3 targetPosition = new Vector3(startX + i * cardSpacing, 0, 0);
                cardUIs[i].transform.localPosition = targetPosition;
                cardUIs[i].SetSortingOrder(i);
            }
        }
    }
    
    public List<Card> GetCardsInHand()
    {
        return new List<Card>(cardsInHand);
    }
    
    public bool HasCard(Card card)
    {
        return cardsInHand.Contains(card);
    }
    
    public CardUI GetCardUI(Card card)
    {
        int index = cardsInHand.IndexOf(card);
        if (index >= 0 && index < cardUIs.Count)
        {
            return cardUIs[index];
        }
        return null;
    }
}