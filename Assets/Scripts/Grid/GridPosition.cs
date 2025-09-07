using UnityEngine;

[System.Serializable]
public class GridPosition
{
    public int x;
    public int y;
    
    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public bool IsCorner()
    {
        return (x == 0 || x == 2) && (y == 0 || y == 2);
    }
    
    public bool IsEdge()
    {
        return x == 0 || x == 2 || y == 0 || y == 2;
    }
    
    public bool IsCenter()
    {
        return x == 1 && y == 1;
    }
    
    public int GetAdjacentCount(GridPosition[,] grid)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                
                int newX = x + dx;
                int newY = y + dy;
                
                if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
                {
                    if (grid[newX, newY] != null) count++;
                }
            }
        }
        return count;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is GridPosition other)
        {
            return x == other.x && y == other.y;
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return x * 10 + y;
    }
}