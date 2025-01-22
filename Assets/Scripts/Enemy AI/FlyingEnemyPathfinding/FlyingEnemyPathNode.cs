using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyPathNode 
{
    private GridSystemGenerics<FlyingEnemyPathNode> grid;
    private int x;
    private int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public FlyingEnemyPathNode cameFromNode;

    public FlyingEnemyPathNode(GridSystemGenerics<FlyingEnemyPathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

}