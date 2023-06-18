using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridIndex : MonoBehaviour
{
    private readonly Dictionary<string, Block> gridIndex = new();

    private string GetKey(Vector2 position)
    {
        return position.x.ToString() + "x" + position.y.ToString();
    }

    public void ChangeIndexPosition(Vector2 oldPosition, Vector2 newPosition, Block newBlock) //used by block
    {
        string oldKey = GetKey(oldPosition);
        if (gridIndex.ContainsKey(oldKey))
            gridIndex.Remove(oldKey);

        gridIndex.Add(GetKey(newPosition), newBlock);
    }

    public Block GetBlockFromIndex(Vector2 position) //used by block
    {
        string key = GetKey(position);
        if (gridIndex.ContainsKey(key))
            return gridIndex[key];
        else
            return null;
    }
}