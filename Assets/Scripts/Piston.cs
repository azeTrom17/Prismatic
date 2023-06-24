using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Gadget
{
    //assigned in prefab
    public string pistonType; //"retractor" or "grappler"
    public Block arm; //read by other Blocks

    //dynamic
    private int stroke = 1; //1 or 2. Stroke 1 extends, 2 retracts/grappples

    [NonSerialized] public bool error; //true when activation failed
    [NonSerialized] public Block errorBlock; //arm or body. If this block would be made a movingBlock, error = true

    [NonSerialized] public List<Block> movingBlocks = new(); //blocks that will move
    //checkingBlocks = number of blocks that are currently checking for neighbors to add
    //to movingBlocks. When checkingBlocks reaches zero, movingBlocks is fully populated
    [NonSerialized] public int checkingBlocks;

    protected override void Start()
    {
        base.Start();
    }

    public override void ActivateGadget(int mouseButton)
    {
        movingBlocks.Clear();

        if (stroke == 1)
        {
            errorBlock = this;

            Block targetBlock = GetTargetBlock();
            if (targetBlock != null)
            {
                //destroy fastener
                Block[] blocks = OrganizedBlocks(this, targetBlock); //organize by blockNumber, lowest first
                string key = GetFastenerKey(blocks[0], blocks[1]);
                DestroyFastener(key);

                //get moving blocks
                checkingBlocks = 1;
                movingBlocks.Add(targetBlock);
                targetBlock.GetMovingBlocks(this);
            }
            else
            {
                checkingBlocks = 0;
                Move();
            }
        }
        //else if (stroke == 2)
        //{
        //    errorBlock = pistonType == "retractor" ? this : arm;

        //    List<Block> neighbors = GetNearbyBlocks();
        //    neighbors.RemoveAt(0); //the first neighbor is the arm
        //    foreach (Block neighbor in neighbors)
        //        if (neighbor == null)
        //            neighbors.Remove(neighbor);
            //multiple checkingBlocks?
        //}
    }

    //true if movingBlock has already been added
    public bool ContainsMovingBlock(Block newBlock) //called by other Blocks
    {
        foreach (Block block in movingBlocks)
            if (block == newBlock)
                return true;
        return false;
    }

    public void Move()
    {
        if (checkingBlocks != 0) return; //movingBlocks not fully populated

        if (error) return;

        stroke = stroke == 1 ? 2 : 1;

        foreach (Block movingBlock in movingBlocks)
        {
            gridIndex.RemoveFromIndex(movingBlock);

            movingBlock.transform.position += transform.up * 1;
        }
        //move all blocks before re-adding to index to prevent errors
        foreach (Block movingBlock in movingBlocks)
            gridIndex.AddToIndex(movingBlock);

        //turn on arm
        arm.transform.position = transform.position + (transform.up * 1);
        arm.gameObject.SetActive(true);
        gridIndex.AddToIndex(arm);

        //fasten arm to its target
        Block armTarget = gridIndex.GetBlockFromIndex(arm.transform.position + (arm.transform.up * 1));
        if (armTarget != null)
        {
            Block[] blocks = OrganizedBlocks(arm, armTarget); //organize by blockNumber, lowest first
            string key = GetFastenerKey(blocks[0], blocks[1]);
            Debug.Log(blocks[0].transform.position);
            Debug.Log(blocks[1].transform.position);
            CreateFastener(key, blocks[0], blocks[1], true);
        }

        //fasten arm and body
        Block[] newBlocks = OrganizedBlocks(this, arm); //organize by blockNumber, lowest first
        string newKey = GetFastenerKey(newBlocks[0], newBlocks[1]);

        CreateFastener(newKey, newBlocks[0], newBlocks[1], false); //no icon
    }
}