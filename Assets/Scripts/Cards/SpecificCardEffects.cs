using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Simple Gold Effect", menuName = "Card Game/Effects/Simple Gold")]
public class SimpleGoldEffect : CardEffect
{
    public SimpleGoldEffect()
    {
        effectName = "Simple Gold";
        baseValue = 10;
        requiresPosition = false;
        requiresFactionCount = false;
    }
}

[CreateAssetMenu(fileName = "Adjacent Bonus Effect", menuName = "Card Game/Effects/Adjacent Bonus")]
public class AdjacentBonusEffect : CardEffect
{
    [Header("Adjacent Settings")]
    public int bonusPerAdjacent = 5;
    
    public AdjacentBonusEffect()
    {
        effectName = "Adjacent Bonus";
        baseValue = 5;
        requiresPosition = true;
        requiresFactionCount = false;
    }
    
    protected override int CalculatePositionBonus(GridPosition position, List<Card> allCardsInGrid)
    {
        CardGrid grid = Object.FindObjectOfType<CardGrid>();
        if (grid == null) return 0;
        
        int adjacentCount = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                
                int newX = position.x + dx;
                int newY = position.y + dy;
                
                if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
                {
                    if (grid.GetCard(new GridPosition(newX, newY)) != null)
                    {
                        adjacentCount++;
                    }
                }
            }
        }
        
        return adjacentCount * bonusPerAdjacent;
    }
}

[CreateAssetMenu(fileName = "Corner Bonus Effect", menuName = "Card Game/Effects/Corner Bonus")]
public class CornerBonusEffect : CardEffect
{
    [Header("Corner Settings")]
    public int cornerBonus = 15;
    
    public CornerBonusEffect()
    {
        effectName = "Corner Bonus";
        baseValue = 8;
        requiresPosition = true;
        requiresFactionCount = false;
    }
    
    protected override int CalculatePositionBonus(GridPosition position, List<Card> allCardsInGrid)
    {
        return position.IsCorner() ? cornerBonus : 0;
    }
}

[CreateAssetMenu(fileName = "Faction Synergy Effect", menuName = "Card Game/Effects/Faction Synergy")]
public class FactionSynergyEffect : CardEffect
{
    [Header("Faction Settings")]
    public int bonusPerSameFaction = 3;
    
    public FactionSynergyEffect()
    {
        effectName = "Faction Synergy";
        baseValue = 6;
        requiresPosition = false;
        requiresFactionCount = true;
    }
    
    protected override int CalculateFactionBonus(int factionCount)
    {
        return (factionCount - 1) * bonusPerSameFaction;
    }
}

[CreateAssetMenu(fileName = "Line Formation Effect", menuName = "Card Game/Effects/Line Formation")]
public class LineFormationEffect : CardEffect
{
    [Header("Line Settings")]
    public int lineBonus = 20;
    
    public LineFormationEffect()
    {
        effectName = "Line Formation";
        baseValue = 5;
        requiresPosition = true;
        requiresFactionCount = false;
    }
    
    protected override int CalculatePositionBonus(GridPosition position, List<Card> allCardsInGrid)
    {
        CardGrid grid = Object.FindObjectOfType<CardGrid>();
        if (grid == null) return 0;
        
        bool horizontalLine = CheckHorizontalLine(position, grid);
        bool verticalLine = CheckVerticalLine(position, grid);
        
        return (horizontalLine || verticalLine) ? lineBonus : 0;
    }
    
    private bool CheckHorizontalLine(GridPosition position, CardGrid grid)
    {
        for (int x = 0; x < 3; x++)
        {
            if (grid.GetCard(new GridPosition(x, position.y)) == null)
                return false;
        }
        return true;
    }
    
    private bool CheckVerticalLine(GridPosition position, CardGrid grid)
    {
        for (int y = 0; y < 3; y++)
        {
            if (grid.GetCard(new GridPosition(position.x, y)) == null)
                return false;
        }
        return true;
    }
}

[CreateAssetMenu(fileName = "Escalating Effect", menuName = "Card Game/Effects/Escalating")]
public class EscalatingEffect : CardEffect
{
    [Header("Escalating Settings")]
    public int bonusPerRound = 2;
    
    public EscalatingEffect()
    {
        effectName = "Escalating";
        baseValue = 3;
        requiresPosition = false;
        requiresFactionCount = false;
    }
    
    public override int CalculateValue(GridPosition position, List<Card> allCardsInGrid, Dictionary<CardFaction, int> factionCounts, List<GlobalModifier> globalModifiers)
    {
        int currentRound = GameManager.Instance ? GameManager.Instance.currentRound : 1;
        int escalatingBonus = (currentRound - 1) * bonusPerRound;
        
        return base.CalculateValue(position, allCardsInGrid, factionCounts, globalModifiers) + escalatingBonus;
    }
}