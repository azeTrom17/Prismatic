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



    [NonSerialized] public int blockNumber; //read by other blocks
    public static int nextAvailableBlockNumber = 0; //only accessed by other blocks

    //each fastener is an array of 2 blocks, with the block with the higher blockNumber first in the array
    public static Dictionary<string, BlockFastener> fasteners = new(); //only accessed by other blocks




    protected GridIndex gridIndex;
    protected ObjectPool objectPool;

    //[NonSerialized] public List<Block> connectedBlocks = new();
    //private int checkingBlocks;

    //basic methods:
    protected void Awake()
    {
        gridIndex = GameObject.FindWithTag("MiscScripts").GetComponent<GridIndex>();
        objectPool = GameObject.FindWithTag("MiscScripts").GetComponent<ObjectPool>();
    }

    protected void Start()
    {
        gridIndex.ChangeIndexPosition(default, transform.position, this);

        blockNumber = nextAvailableBlockNumber;
        nextAvailableBlockNumber++;

        FastenNearbyBlocks();
    }

    //helper methods:
    private List<Block> GetNearbyBlocks() //returns the 4 adjacent blocks (some may be null)
    {
        List<Vector2> nearbyPositions = new()
        {
            transform.position + transform.up * 1,
            transform.position + transform.up * -1,
            transform.position + transform.right * 1,
            transform.position + transform.right * -1
        };

        List<Block> nearbyBlocks = new();
        foreach (Vector2 pos in nearbyPositions)
            nearbyBlocks.Add(gridIndex.GetBlockFromIndex(pos));

        return nearbyBlocks;
    }

    protected Block[] OrganizedBlocks(Block block1, Block block2) //organizes two blocks by blockNumber (places lower number first)
    {
        Block[] organizedBlocks = new Block[2];
        organizedBlocks[0] = block1.blockNumber < block2.blockNumber ? block1 : block2;
        organizedBlocks[1] = organizedBlocks[0] == block1 ? block2 : block1;
        return organizedBlocks;
    }

    protected string GetFastenerKey(Block block1, Block block2)
    {
        return block1.blockNumber + "x" + block2.blockNumber;
    }

    protected void CreateFastener(string key, Block lowerBlock, Block higherBlock)
    {
        GameObject fastenerIcon = objectPool.GetPooledFastener();
        fastenerIcon.transform.SetParent(transform);
        fastenerIcon.transform.position = (lowerBlock.transform.position + higherBlock.transform.position) / 2; //midpoint
        fastenerIcon.SetActive(true);

        BlockFastener blockFastener = new()
        {
            lowerBlock = lowerBlock,
            higherBlock = higherBlock,
            fastenerIcon = fastenerIcon
        };

        fasteners.Add(key, blockFastener);
    }

    protected void DestroyFastener(string key)
    {
        //return fastenerIcon to pool
        GameObject fastenerIcon = fasteners[key].fastenerIcon;
        fastenerIcon.transform.SetParent(objectPool.poolParent);
        fastenerIcon.SetActive(false);

        //destroy fastener
        fasteners.Remove(key);
    }

    //misc methods:
    private void FastenNearbyBlocks()
    {
        foreach (Block neighbor in GetNearbyBlocks())
        {
            if (neighbor == null) continue;

            Block[] blocks = OrganizedBlocks(this, neighbor); //organize by blockNumber
            Block lowerBlock = blocks[0]; //block with lower blockNumber
            Block higherBlock = blocks[1];

            string key = GetFastenerKey(lowerBlock, higherBlock);

            if (fasteners.ContainsKey(key)) continue;

            CreateFastener(key, lowerBlock, higherBlock);
        }
    }























    //public bool ContainsConnectedBlock(Block newBlock) //called by other Blocks
    //{
    //    foreach (Block block in connectedBlocks)
    //        if (block == newBlock)
    //            return true;
    //    return false;
    //}

    //public void GetNearbyBlocks(Block original)
    //{
    //    if (original == this)
    //    {
    //        checkingBlocks = 1;
    //        connectedBlocks.Clear();
    //        connectedBlocks.Add(this);
    //    }

    //    List<Vector2> nearbyPositions = new()
    //    {
    //        transform.position + transform.up * 1,
    //        transform.position + transform.up * -1,
    //        transform.position + transform.right * 1,
    //        transform.position + transform.right * -1
    //    };

    //    foreach (Vector2 vector2 in nearbyPositions)
    //    {
    //        Block neighbor = gridIndex.GetBlockFromIndex(vector2);
    //        if (neighbor != null && !original.ContainsConnectedBlock(neighbor))
    //        {
    //            original.ChangeCheckingBlocks(1); //add one before calling GetNearbyBlocks in neighbor

    //            original.connectedBlocks.Add(neighbor);
    //            neighbor.GetNearbyBlocks(original);
    //        }
    //    }
    //    original.ChangeCheckingBlocks(-1);
    //}
    //public void ChangeCheckingBlocks(int amount) //change name, add comments
    //{
    //    checkingBlocks += amount;

    //    if (checkingBlocks == 0)
    //        Debug.Log("finished checking!");
    //}

    //private void MoveBlock()
    //{
    //    Vector2 targetMovePosition = transform.position;

    //    if (gridIndex.GetBlockFromIndex(targetMovePosition) == null)
    //    {
    //        gridIndex.ChangeIndexPosition(transform.position, targetMovePosition, this);
    //        transform.position = targetMovePosition;
    //    }
    //}

    public void DestroyBlock() //called by Bomb
    {
        foreach (Block neighbor in GetNearbyBlocks())
        {
            if (neighbor == null) continue;

            Block[] blocks = OrganizedBlocks(this, neighbor); //organize by blockNumber
            string key = GetFastenerKey(blocks[0], blocks[1]);
            if (fasteners.ContainsKey(key))
                DestroyFastener(key);
        }

        //if piston, destroy both arm and body
        Destroy(gameObject);
    }
}
public struct BlockFastener
{
    public Block lowerBlock; //lowerBlock = block with lower blockNumber
    public Block higherBlock;

    public GameObject fastenerIcon;
}