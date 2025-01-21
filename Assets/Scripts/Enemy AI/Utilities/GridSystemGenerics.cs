using System;
using System.Numerics;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GridSystemGenerics<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    private UnityEngine.Vector3 originPosition;
    private bool showDebug;

    public GridSystemGenerics(int width, int height, float cellSize, UnityEngine.Vector3 originPosition, bool showDebug, Func<TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition =  originPosition;
        this.showDebug = showDebug;
    

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        //initialize Generic Standard Type (since it's not always null)
        for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject();
                }
            }

        if (showDebug)
        {
            //loop through each cell and draw it
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x,y] = Utils.CreateWorldText(null, gridArray[x,y]?.ToString(), GetWorldPosition(x,y) + new UnityEngine.Vector3(cellSize, cellSize) * 0.5f, 10, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }

                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            }
        }
    }

    private UnityEngine.Vector3 GetWorldPosition(int x, int y)
    {
        return new UnityEngine.Vector3(x,y) * cellSize + originPosition;
    }

    private void GetXY(UnityEngine.Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);

    }
    
    public void SetGridObject(int x, int y, TGridObject value)
    {  
        //ignore values that don't make sense or are negative
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x,y].text = gridArray[x,y].ToString();
        }
    }

    public void SetGridObject (UnityEngine.Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x,y];
        }
        else
        {
            return default(TGridObject);
        }
            
    }

    public TGridObject GetGridObject(UnityEngine.Vector3 worldPosition)
    {
        int x,y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }


}
