using UnityEngine;

public class Cell
{
    public int x;
    public int y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }   

    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isDiggable;

}
