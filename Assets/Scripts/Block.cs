using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //4 steps:
    //1. At game start, get all connected blocks and fasten them. (what's the original?)
    //2. When piston/hinge demands it, blocks must get all connected and fastened blocks
    //3. Then, those blocks must check to see if they can move to the target position
    //4. Finally, if the blocks can move, they lerp to the position. Else, error




    protected GridIndex gridIndex;
    protected ObjectPool objectPool;

    [NonSerialized] public List<Block> connectedBlocks = new();
    private int checkingBlocks;

    protected void Awake()
    {
        gridIndex = GameObject.FindWithTag("MiscScripts").GetComponent<GridIndex>();
        objectPool = GameObject.FindWithTag("MiscScripts").GetComponent<ObjectPool>();
    }

    protected void Start()
    {
        gridIndex.ChangeIndexPosition(default, transform.position, this);
    }

    public bool ContainsConnectedBlock(Block newBlock) //called by other Blocks
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
                original.ChangeCheckingBlocks(1); //add one before calling GetNearbyBlocks in neighbor

                original.connectedBlocks.Add(neighbor);
                neighbor.GetNearbyBlocks(original);
            }
        }
        original.ChangeCheckingBlocks(-1);
    }
    public void ChangeCheckingBlocks(int amount) //change name, add comments
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

    public void DestroyBlock() //called by Bomb
    {
        //destroy fasteners
        //if piston, destroy both arm and body
        Destroy(gameObject);
    }
}
public class BlockFastener
{
    public Block block1;
    public Block block2;
}