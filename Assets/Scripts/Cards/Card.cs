using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum CardFaction
{
    Nature,
    Technology,
    Magic,
    Commerce,
    Military
}

[System.Serializable]
public class CardEffect
{
    public string effectName;
    public int baseValue;
    public bool requiresPosition;
    public bool requiresFactionCount;
    public bool isGlobalModifier;
    
    public virtual int CalculateValue(GridPosition position, List<Card> allCardsInGrid, Dictionary<CardFaction, int> factionCounts, List<GlobalModifier> globalModifiers)
    {
        int finalValue = baseValue;
        
        if (requiresPosition)
        {
            finalValue += CalculatePositionBonus(position, allCardsInGrid);
        }
        
        if (requiresFactionCount && factionCounts.ContainsKey(GetCard().faction))
        {
            finalValue += CalculateFactionBonus(factionCounts[GetCard().faction]);
        }
        
        foreach (var modifier in globalModifiers)
        {
            finalValue = modifier.ApplyModifier(finalValue, GetCard());
        }
        
        return finalValue;
    }
    
    protected virtual int CalculatePositionBonus(GridPosition position, List<Card> allCardsInGrid)
    {
        return 0;
    }
    
    protected virtual int CalculateFactionBonus(int factionCount)
    {
        return (factionCount - 1) * 2;
    }
    
    protected virtual Card GetCard()
    {
        return null;
    }
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card")]
public class Card : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    public string description;
    public Sprite cardArt;
    public CardFaction faction;
    public int cost;
    public int rarity;
    
    [Header("Effects")]
    public List<CardEffect> effects = new List<CardEffect>();
    
    [Header("Positioning")]
    public bool prefersCorners;
    public bool prefersEdges;
    public bool prefersCenter;
    
    public int CalculateTotalGold(GridPosition position, List<Card> allCardsInGrid, Dictionary<CardFaction, int> factionCounts, List<GlobalModifier> globalModifiers)
    {
        int totalGold = 0;
        
        foreach (var effect in effects)
        {
            totalGold += effect.CalculateValue(position, allCardsInGrid, factionCounts, globalModifiers);
        }
        
        return totalGold;
    }
}