using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridIndex : MonoBehaviour
{
    private readonly Dictionary<string, Block> gridIndex = new();

    private string GetIndexKey(Vector2 position)
    {
        int xPos = Mathf.RoundToInt(position.x); //must round to prevent errors
        int yPos = Mathf.RoundToInt(position.y); //(positions are sometimes millimeters off)
        return xPos + "x" + yPos;
    }

    public void AddToIndex(Block block)
    {
        gridIndex.Add(GetIndexKey(block.transform.position), block);
    }
    
    public void RemoveFromIndex(Block block)
    {
        gridIndex.Remove(GetIndexKey(block.transform.position));
    }

    public Block GetBlockFromIndex(Vector2 position)
    {
        string key = GetIndexKey(position);
        if (gridIndex.ContainsKey(key))
            return gridIndex[key];
        else
            return null;
    }
}