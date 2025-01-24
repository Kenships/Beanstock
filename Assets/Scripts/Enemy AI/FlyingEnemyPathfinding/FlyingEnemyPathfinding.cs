using System.Collections.Generic;
using System.IO;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class FlyingEnemyPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
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
        FlyingEnemyPathNode endNode = grid.GetGridObject(endX, endY);

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
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        //cycle

        //while we still have nodes in the open list
        while (openList.Count > 0)
        {
            FlyingEnemyPathNode currentNode = GetLowestFCostNode(openList);
            //check if it is the final node
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            // if it is not the end node, move from open to closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //cycle through neighbours of the current node
            foreach(FlyingEnemyPathNode neighbourNode in GetNeighbourList(currentNode))
            {
                
            }
        }
    }

    
    //return calculated path to the end node
    private List<FlyingEnemyPathNode> CalculatePath(FlyingEnemyPathNode endNode)
    {return null;}
    
    //Quickest direct path
    private int CalculateDistanceCost(FlyingEnemyPathNode a, FlyingEnemyPathNode b)
    {
        int xDistance = Mathf.Abs(a.x -b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST*remaining;
    }

    private FlyingEnemyPathNode GetLowestFCostNode(List<FlyingEnemyPathNode> pathNodeList)
    {
        FlyingEnemyPathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<FlyingEnemyPathNode> GetNeighbourList(FlyingEnemyPathNode currentNode)
    {
        List<FlyingEnemyPathNode> neighbourList = new List<FlyingEnemyPathNode>();

        if (currentNode.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));

            //Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y -1));

            //Left Up
            if (currentNode.y + 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));

            
        }
        if (currentNode.x + 1 >= 0)
        {
            //Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));

            //Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y -1));

            //Right Up
            if (currentNode.y + 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));   
        }
        
        //Down
        if (currentNode.y - 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private FlyingEnemyPathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }



}