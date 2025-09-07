using UnityEngine;

[System.Serializable]
public abstract class GlobalModifier : ScriptableObject
{
    [Header("Modifier Info")]
    public string modifierName;
    public string description;
    public Sprite icon;
    public bool isTemporary;
    public int remainingTurns;
    
    public abstract int ApplyModifier(int baseValue, Card card);
    
    public virtual bool ShouldApplyToCard(Card card)
    {
        return true;
    }
    
    public virtual void OnTurnStart()
    {
        if (isTemporary && remainingTurns > 0)
        {
            remainingTurns--;
        }
    }
    
    public virtual void OnTurnEnd()
    {
        
    }
    
    public bool IsExpired()
    {
        return isTemporary && remainingTurns <= 0;
    }
}

[CreateAssetMenu(fileName = "New Faction Modifier", menuName = "Card Game/Modifiers/Faction Modifier")]
public class FactionModifier : GlobalModifier
{
    [Header("Faction Settings")]
    public CardFaction targetFaction;
    public int bonusMultiplier = 2;
    public bool isPercentage = false;
    
    public override int ApplyModifier(int baseValue, Card card)
    {
        if (card.faction != targetFaction)
            return baseValue;
            
        if (isPercentage)
        {
            return baseValue + Mathf.RoundToInt(baseValue * (bonusMultiplier / 100f));
        }
        else
        {
            return baseValue + bonusMultiplier;
        }
    }
    
    public override bool ShouldApplyToCard(Card card)
    {
        return card.faction == targetFaction;
    }
}

[CreateAssetMenu(fileName = "New Position Modifier", menuName = "Card Game/Modifiers/Position Modifier")]
public class PositionModifier : GlobalModifier
{
    [Header("Position Settings")]
    public bool affectsCorners;
    public bool affectsEdges;
    public bool affectsCenter;
    public int bonusAmount = 5;
    
    public override int ApplyModifier(int baseValue, Card card)
    {
        return baseValue + bonusAmount;
    }
}

[CreateAssetMenu(fileName = "New Global Multiplier", menuName = "Card Game/Modifiers/Global Multiplier")]
public class GlobalMultiplier : GlobalModifier
{
    [Header("Multiplier Settings")]
    public float multiplier = 1.5f;
    public int maxBonus = 50;
    
    public override int ApplyModifier(int baseValue, Card card)
    {
        int bonus = Mathf.RoundToInt(baseValue * (multiplier - 1f));
        return baseValue + Mathf.Min(bonus, maxBonus);
    }
}