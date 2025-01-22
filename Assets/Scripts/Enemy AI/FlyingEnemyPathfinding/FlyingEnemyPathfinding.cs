using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyPathfinding
{

    private GridSystemGenerics<FlyingEnemyPathNode> grid;
    private List<FlyingEnemyPathNode> openList;
    private List<FlyingEnemyPathNode> closedList;
    public FlyingEnemyPathfinding(int width, int height)
    {
        grid = new GridSystemGenerics<FlyingEnemyPathNode>(width, height, 5f, Vector3.zero, true, (GridSystemGenerics<FlyingEnemyPathNode> g, int x, int y) => new FlyingEnemyPathNode(g, x, y));
    }

    private List<FlyingEnemyPathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        FlyingEnemyPathNode startNode = grid.GetGridObject(startX, startY);

        openList = new List<FlyingEnemyPathNode>{ startNode };
        closedList = new List<FlyingEnemyPathNode>{};

        //cycle through nodes, set g cost to inf, calculate f cost
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                FlyingEnemyPathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null; 
            }
        }

        startNode.gCost = 0;
    }

    private int CalculateDistance(FlyingEnemyPathNode a,  )





}