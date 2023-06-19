using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private GridIndex gridIndex;

    [NonSerialized] public List<Block> connectedBlocks = new();
    private int checkingBlocks;

    private void Awake()
    {
        gridIndex = GameObject.FindWithTag("MiscScripts").GetComponent<GridIndex>();
    }

    private void Start()
    {
        gridIndex.ChangeIndexPosition(default, transform.position, this);
    }

    public bool ContainsConnectedBlock(Block newBlock) //true if block is already in connectedBlocks
    {
        foreach (Block block in connectedBlocks)
            if (block == newBlock)
                return true;
        return false;
    }

    public void GetNearbyBlocks(Block original)
    {
        if (original == this)
        {
            checkingBlocks = 1;
            connectedBlocks.Clear();
            connectedBlocks.Add(this);
        }

        List<Vector2> nearbyPositions = new()
        {
            transform.position + transform.up * 1,
            transform.position + transform.up * -1,
            transform.position + transform.right * 1,
            transform.position + transform.right * -1
        };

        foreach (Vector2 vector2 in nearbyPositions)
        {
            Block neighbor = gridIndex.GetBlockFromIndex(vector2);
            if (neighbor != null && !original.ContainsConnectedBlock(neighbor))
            {
                original.ChangeCheckingBlocks(1, name); //add one before calling GetNearbyBlocks in neighbor

                original.connectedBlocks.Add(neighbor);
                neighbor.GetNearbyBlocks(original);
            }
        }
        original.ChangeCheckingBlocks(-1, name);
    }
    public void ChangeCheckingBlocks(int amount, string name) //change name, add comments
    {
        checkingBlocks += amount;

        if (checkingBlocks == 0)
            Debug.Log("finished checking!");
    }

    private void MoveBlock()
    {
        Vector2 targetMovePosition = transform.position;

        if (gridIndex.GetBlockFromIndex(targetMovePosition) == null)
        {
            gridIndex.ChangeIndexPosition(transform.position, targetMovePosition, this);
            transform.position = targetMovePosition;
        }
    }
}
public class BlockFastener
{
    public Block block1;
    public Block block2;
}