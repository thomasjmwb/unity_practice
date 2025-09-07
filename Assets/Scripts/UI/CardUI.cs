using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image cardImage;
    public Image artImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Image factionIcon;
    
    [Header("Drag Settings")]
    public float dragScale = 1.2f;
    public float hoverScale = 1.1f;
    
    private Card cardData;
    private HandManager handManager;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isDragging;
    private int originalSortingOrder;
    
    public Card CardData => cardData;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvas = GetComponentInParent<Canvas>();
        originalScale = transform.localScale;
        
        SetupUIComponents();
    }
    
    void SetupUIComponents()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();
            
        if (nameText == null)
            nameText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void Initialize(Card card, HandManager manager)
    {
        cardData = card;
        handManager = manager;
        UpdateCardDisplay();
    }
    
    void UpdateCardDisplay()
    {
        if (cardData == null) return;
        
        if (nameText != null)
            nameText.text = cardData.cardName;
            
        if (costText != null)
            costText.text = cardData.cost.ToString();
            
        if (descriptionText != null)
            descriptionText.text = cardData.description;
            
        if (artImage != null && cardData.cardArt != null)
            artImage.sprite = cardData.cardArt;
            
        if (factionIcon != null)
        {
            Color factionColor = GetFactionColor(cardData.faction);
            factionIcon.color = factionColor;
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
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardData == null) return;
        
        isDragging = true;
        originalPosition = transform.position;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        
        transform.localScale = originalScale * dragScale;
        SetSortingOrder(1000);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            transform.position = eventData.position;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        transform.position = originalPosition;
        transform.localScale = originalScale;
        SetSortingOrder(originalSortingOrder);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
        {
            transform.localScale = originalScale * hoverScale;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            transform.localScale = originalScale;
        }
    }
    
    public bool TryPlaceOnGrid(GridPosition position)
    {
        if (cardData == null || handManager == null) return false;
        
        CardGrid grid = FindObjectOfType<CardGrid>();
        if (grid == null) return false;
        
        if (grid.PlaceCard(cardData, position))
        {
            handManager.RemoveCardFromHand(cardData);
            return true;
        }
        
        return false;
    }
    
    public void SetSortingOrder(int order)
    {
        if (order == -1)
        {
            order = originalSortingOrder;
        }
        else if (originalSortingOrder == 0)
        {
            originalSortingOrder = order;
        }
        
        Canvas cardCanvas = GetComponent<Canvas>();
        if (cardCanvas == null)
        {
            cardCanvas = gameObject.AddComponent<Canvas>();
            cardCanvas.overrideSorting = true;
        }
        
        cardCanvas.sortingOrder = order;
    }
}