using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Components")]
    public Image backgroundImage;
    public Image cardImage;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;
    public Color occupiedColor = Color.gray;
    
    private GridPosition gridPosition;
    private CardGrid cardGrid;
    private Card currentCard;
    private bool isHighlighted;
    
    public GridPosition Position => gridPosition;
    public bool IsOccupied => currentCard != null;
    public Card CurrentCard => currentCard;
    
    void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
            
        if (cardImage == null)
        {
            // Check if we have any children before trying to access them
            if (transform.childCount > 0)
            {
                cardImage = transform.GetChild(0)?.GetComponent<Image>();
            }
            
            // If still null, create a new card image object
            if (cardImage == null)
            {
                GameObject cardImageObj = new GameObject("CardImage");
                cardImageObj.transform.SetParent(transform);
                cardImageObj.transform.localPosition = Vector3.zero;
                cardImageObj.transform.localScale = Vector3.one;
                cardImage = cardImageObj.AddComponent<Image>();
            }
        }
        
        UpdateVisuals();
    }
    
    public void Initialize(GridPosition position, CardGrid grid)
    {
        gridPosition = position;
        cardGrid = grid;
        currentCard = null;
        UpdateVisuals();
    }
    
    public void SetCard(Card card)
    {
        currentCard = card;
        if (cardImage != null && card != null)
        {
            cardImage.sprite = card.cardArt;
            cardImage.enabled = true;
        }
        UpdateVisuals();
    }
    
    public void ClearCard()
    {
        currentCard = null;
        if (cardImage != null)
        {
            cardImage.sprite = null;
            cardImage.enabled = false;
        }
        UpdateVisuals();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        CardUI draggedCard = eventData.pointerDrag?.GetComponent<CardUI>();
        if (draggedCard != null && !IsOccupied)
        {
            draggedCard.TryPlaceOnGrid(gridPosition);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsOccupied)
        {
            isHighlighted = true;
            UpdateVisuals();
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted = false;
        UpdateVisuals();
    }
    
    void UpdateVisuals()
    {
        if (backgroundImage == null) return;
        
        if (IsOccupied)
        {
            backgroundImage.color = occupiedColor;
        }
        else if (isHighlighted)
        {
            backgroundImage.color = highlightColor;
        }
        else
        {
            backgroundImage.color = normalColor;
        }
    }
}