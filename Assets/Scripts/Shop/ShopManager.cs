using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ShopItem
{
    public Card card;
    public int price;
    public int stock;
    public bool isUnlimited;
    
    public ShopItem(Card card, int price, int stock = 1, bool isUnlimited = false)
    {
        this.card = card;
        this.price = price;
        this.stock = stock;
        this.isUnlimited = isUnlimited;
    }
}

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    public List<Card> availableCards = new List<Card>();
    public int cardsPerShop = 6;
    public float priceMultiplier = 1.0f;
    public Transform shopParent;
    public GameObject shopItemPrefab;
    
    [Header("Shop Progression")]
    public List<Card> tier1Cards = new List<Card>();
    public List<Card> tier2Cards = new List<Card>();
    public List<Card> tier3Cards = new List<Card>();
    
    private List<ShopItem> currentShopItems = new List<ShopItem>();
    private List<ShopItemUI> shopItemUIs = new List<ShopItemUI>();
    private int playerGold;
    private bool isShopOpen;
    
    public System.Action<ShopItem> OnItemPurchased;
    public System.Action OnShopClosed;
    public System.Action<int> OnGoldChanged;
    
    void Start()
    {
        if (shopParent == null)
            shopParent = transform;
    }
    
    public void OpenShop(int gold)
    {
        playerGold = gold;
        isShopOpen = true;
        GenerateShopItems();
        CreateShopUI();
        OnGoldChanged?.Invoke(playerGold);
    }
    
    public void CloseShop()
    {
        isShopOpen = false;
        ClearShopUI();
        
        GameManager.Instance?.FinishShopping(playerGold);
        OnShopClosed?.Invoke();
    }
    
    void GenerateShopItems()
    {
        currentShopItems.Clear();
        
        List<Card> availableForShop = GetAvailableCardsForCurrentRound();
        
        for (int i = 0; i < cardsPerShop && i < availableForShop.Count; i++)
        {
            Card randomCard = availableForShop[Random.Range(0, availableForShop.Count)];
            availableForShop.Remove(randomCard);
            
            int price = CalculateCardPrice(randomCard);
            ShopItem item = new ShopItem(randomCard, price, 1, false);
            currentShopItems.Add(item);
        }
    }
    
    List<Card> GetAvailableCardsForCurrentRound()
    {
        List<Card> available = new List<Card>();
        int currentRound = GameManager.Instance ? GameManager.Instance.currentRound : 1;
        
        if (currentRound <= 4)
        {
            available.AddRange(tier1Cards);
        }
        else if (currentRound <= 8)
        {
            available.AddRange(tier1Cards);
            available.AddRange(tier2Cards);
        }
        else
        {
            available.AddRange(tier1Cards);
            available.AddRange(tier2Cards);
            available.AddRange(tier3Cards);
        }
        
        if (available.Count == 0)
        {
            available.AddRange(availableCards);
        }
        
        return available;
    }
    
    int CalculateCardPrice(Card card)
    {
        int basePrice = card.cost * 10;
        basePrice += card.rarity * 5;
        return Mathf.RoundToInt(basePrice * priceMultiplier);
    }
    
    void CreateShopUI()
    {
        ClearShopUI();
        
        for (int i = 0; i < currentShopItems.Count; i++)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopParent);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            
            if (itemUI == null)
            {
                itemUI = itemObj.AddComponent<ShopItemUI>();
            }
            
            itemUI.Initialize(currentShopItems[i], this);
            shopItemUIs.Add(itemUI);
            
            Vector3 position = CalculateItemPosition(i);
            itemObj.transform.localPosition = position;
        }
    }
    
    Vector3 CalculateItemPosition(int index)
    {
        int columns = 3;
        int row = index / columns;
        int col = index % columns;
        
        return new Vector3(col * 200f - 200f, -row * 250f, 0);
    }
    
    void ClearShopUI()
    {
        foreach (ShopItemUI itemUI in shopItemUIs)
        {
            if (itemUI != null)
            {
                Destroy(itemUI.gameObject);
            }
        }
        shopItemUIs.Clear();
    }
    
    public bool TryPurchaseItem(ShopItem item)
    {
        if (!isShopOpen || playerGold < item.price || item.stock <= 0)
        {
            return false;
        }
        
        playerGold -= item.price;
        
        if (!item.isUnlimited)
        {
            item.stock--;
        }
        
        GameManager.Instance?.playerDeck.AddCardToDeck(item.card);
        
        OnItemPurchased?.Invoke(item);
        OnGoldChanged?.Invoke(playerGold);
        
        UpdateShopUI();
        
        return true;
    }
    
    void UpdateShopUI()
    {
        foreach (ShopItemUI itemUI in shopItemUIs)
        {
            if (itemUI != null)
            {
                itemUI.UpdateDisplay();
            }
        }
    }
    
    public void RefreshShop(int cost)
    {
        if (playerGold < cost) return;
        
        playerGold -= cost;
        GenerateShopItems();
        CreateShopUI();
        OnGoldChanged?.Invoke(playerGold);
    }
    
    public bool CanAfford(int price)
    {
        return playerGold >= price;
    }
}