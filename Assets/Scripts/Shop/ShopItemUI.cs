using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image cardImage;
    public Image artImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI stockText;
    public TextMeshProUGUI descriptionText;
    public Button purchaseButton;
    public Image factionIcon;
    
    [Header("Visual States")]
    public Color affordableColor = Color.white;
    public Color unaffordableColor = Color.gray;
    public Color outOfStockColor = Color.red;
    
    private ShopItem shopItem;
    private ShopManager shopManager;
    
    void Awake()
    {
        SetupUIComponents();
        
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }
    
    void SetupUIComponents()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();
            
        if (purchaseButton == null)
            purchaseButton = GetComponentInChildren<Button>();
    }
    
    public void Initialize(ShopItem item, ShopManager manager)
    {
        shopItem = item;
        shopManager = manager;
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        if (shopItem?.card == null) return;
        
        Card card = shopItem.card;
        
        if (nameText != null)
            nameText.text = card.cardName;
            
        if (priceText != null)
            priceText.text = $"{shopItem.price}g";
            
        if (stockText != null)
        {
            if (shopItem.isUnlimited)
            {
                stockText.text = "∞";
            }
            else
            {
                stockText.text = shopItem.stock.ToString();
            }
        }
        
        if (descriptionText != null)
            descriptionText.text = card.description;
            
        if (artImage != null && card.cardArt != null)
            artImage.sprite = card.cardArt;
            
        if (factionIcon != null)
        {
            Color factionColor = GetFactionColor(card.faction);
            factionIcon.color = factionColor;
        }
        
        UpdateButtonState();
    }
    
    void UpdateButtonState()
    {
        if (purchaseButton == null || shopManager == null) return;
        
        bool canAfford = shopManager.CanAfford(shopItem.price);
        bool inStock = shopItem.stock > 0 || shopItem.isUnlimited;
        bool canPurchase = canAfford && inStock;
        
        purchaseButton.interactable = canPurchase;
        
        if (cardImage != null)
        {
            if (!inStock)
            {
                cardImage.color = outOfStockColor;
            }
            else if (!canAfford)
            {
                cardImage.color = unaffordableColor;
            }
            else
            {
                cardImage.color = affordableColor;
            }
        }
        
        if (priceText != null)
        {
            priceText.color = canAfford ? Color.green : Color.red;
        }
    }
    
    void OnPurchaseClicked()
    {
        if (shopManager != null && shopItem != null)
        {
            bool success = shopManager.TryPurchaseItem(shopItem);
            
            if (success)
            {
                Debug.Log($"Purchased {shopItem.card.cardName} for {shopItem.price} gold");
            }
            else
            {
                Debug.Log($"Failed to purchase {shopItem.card.cardName}");
            }
        }
    }
    
    Color GetFactionColor(CardFaction faction)
    {
        switch (faction)
        {
            case CardFaction.Nature: return Color.green;
            case CardFaction.Technology: return Color.blue;
            case CardFaction.Magic: return Color.magenta;
            case CardFaction.Commerce: return Color.yellow;
            case CardFaction.Military: return Color.red;
            default: return Color.white;
        }
    }
}