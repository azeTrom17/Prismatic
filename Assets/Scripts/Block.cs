using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.UI.Image;

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

    protected virtual void Start()
    {
        gridIndex.AddToIndex(this);

        blockNumber = nextAvailableBlockNumber;
        nextAvailableBlockNumber++;

        FastenNearbyBlocks();
    }

    //helper methods:
    protected List<Block> GetNearbyBlocks() //returns the 4 adjacent blocks (some may be null). Used by pistons
    {
        List<Vector2> nearbyPositions = new()
        {
            //place up block first in list so that pistons can easily remove it
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

    protected void CreateFastener(string key, Block lowerBlock, Block higherBlock, bool createIcon)
    {
        GameObject fastenerIcon = null;
        if (createIcon) //piston bodies are fastened to arms but don't have icons
        {
            fastenerIcon = objectPool.GetPooledFastener();
            fastenerIcon.transform.SetParent(transform);
            fastenerIcon.transform.position = (lowerBlock.transform.position + higherBlock.transform.position) / 2; //midpoint
            fastenerIcon.SetActive(true);
        }

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
        if (fastenerIcon != null) //piston bodies are fastened to arms but don't have icons
        {
            fastenerIcon.transform.SetParent(objectPool.poolParent);
            fastenerIcon.SetActive(false);
        }

        //destroy fastener
        fasteners.Remove(key);
    }

    //misc methods:
    public void FastenNearbyBlocks()
    {
        foreach (Block neighbor in GetNearbyBlocks())
        {
            if (neighbor == null) continue;

            Block[] blocks = OrganizedBlocks(this, neighbor); //organize by blockNumber, lowest first

            string key = GetFastenerKey(blocks[0], blocks[1]);

            if (fasteners.ContainsKey(key)) continue;

            CreateFastener(key, blocks[0], blocks[1], true);
        }
    }

    public void GetMovingBlocks(Piston piston) //used only by pistons
    {
        //if there's a wall in the direction of piston's transform.up, tell piston error and return

        List<Block> neighbors = GetNearbyBlocks();
        foreach (Block neighbor in neighbors)
        {
            if (neighbor == null) continue;

            if (piston.ContainsMovingBlock(neighbor)) continue;

            Block[] blocks = OrganizedBlocks(this, neighbor); //organize by blockNumber, lowest first
            string key = GetFastenerKey(blocks[0], blocks[1]);
            bool neighborIsFastened = fasteners.ContainsKey(key);

            Vector2 neighborDirection = (neighbor.transform.position - transform.position).normalized;
            bool neighborIsInFront = neighborDirection == (Vector2)piston.transform.up;

            if (!neighborIsFastened && !neighborIsInFront) continue; //neighbor must be either fastened or in front

            //check this last. If any of the above checks fail, no need to cause an error
            if (neighbor == piston.errorBlock)
            {
                piston.error = true;
                piston.checkingBlocks -= 1;
                piston.Move();
                return;
            }

            piston.checkingBlocks += 1;
            piston.movingBlocks.Add(neighbor);
            neighbor.GetMovingBlocks(piston);
        }

        piston.checkingBlocks -= 1;
        piston.Move(); //will fail to move if checkingBlocks is not yet 0
    }

    public void DestroyBlock() //called by Bomb
    {
        foreach (Block neighbor in GetNearbyBlocks())
        {
            if (neighbor == null) continue;

            Block[] blocks = OrganizedBlocks(this, neighbor); //organize by blockNumber, lowest first
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