using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Gadget
{
    //assigned in prefab
    public string pistonType; //"retractor" or "grappler"
    public Block arm; //read by other Blocks
    public SpriteRenderer sr;
    public Sprite bodySprite;
    public Sprite extendedBodySprite;

    //dynamic
    private enum Action { extending, retracting, grappling};
    private Action action;

    private Block targetBlock; //used for extending/retracting
    private bool targetIsFastened;

    //dynamic + accessed by Block
    [NonSerialized] public bool error; //true when activation failed
    [NonSerialized] public Block errorBlock; //arm or body. If this block would be made a movingBlock, error = true
    [NonSerialized] public List<Block> movingBlocks = new(); //blocks that will move
    //checkingBlocks = number of blocks that are currently checking for neighbors to add
    //to movingBlocks. When checkingBlocks reaches zero, movingBlocks is fully populated
    [NonSerialized] public int checkingBlocks;

    public override void ActivateGadget(int mouseButton)
    {
        if (action == Action.extending || action == Action.retracting)
        {
            errorBlock = this;

            targetBlock = gridIndex.GetBlockFromIndex(arm.transform.position + transform.up * 1);
            if (targetBlock != null)
            {
                //unfasten target block:
                Block targetFastenedPart = action == Action.extending ? this : arm;

                string key = GetFastenerKey(targetFastenedPart, targetBlock);
                targetIsFastened = fasteners.ContainsKey(key); //if true, target will be re-fastened after moving

                if (action == Action.retracting && !targetIsFastened) //can't retract an unfastened target
                {
                    checkingBlocks = 0;
                    ReadyForAction();
                    return;
                }

                ChangeFastening(targetFastenedPart, targetBlock, false);

                //get moving blocks
                checkingBlocks = 1;
                movingBlocks.Add(targetBlock);
                targetBlock.GetMovingBlocks(this);
            }
            else
            {
                checkingBlocks = 0;
                ReadyForAction();
            }
        }
        else //if grappling
        {
            errorBlock = arm;

            //unfasten body and arm
            ChangeFastening(this, arm, false);

            checkingBlocks = 1;
            movingBlocks.Add(this);
            GetMovingBlocks(this);
        }
    }

    //true if movingBlock has already been added
    public bool ContainsMovingBlock(Block newBlock) //called by other Blocks
    {
        foreach (Block block in movingBlocks)
            if (block == newBlock)
                return true;
        return false;
    }

    public void ReadyForAction()
    {
        if (checkingBlocks != 0) return; //movingBlocks not fully populated

        if (error)
        {
            //re-fasten target if it was previously fastened
            if (targetBlock != null && targetIsFastened)
            {
                Block targetFastenedPart = action == Action.extending ? this : arm;
                ChangeFastening(targetFastenedPart, targetBlock, true);
            }

            Reset(); //must reset to prevent bugs

            return;
        }


        //get move direction
        Vector2 moveDirection = transform.up;
        if (action == Action.retracting)
            moveDirection *= -1;


        //moving:
        //if extending, unparent arm
        if (action == Action.extending)
            arm.transform.SetParent(transform.parent);

        //remove moving blocks from index
        foreach (Block movingBlock in movingBlocks)
            gridIndex.RemoveFromIndex(movingBlock);
        if (action != Action.extending)
            gridIndex.RemoveFromIndex(arm);

        //move blocks
        foreach (Block movingBlock in movingBlocks)
            movingBlock.transform.position += (Vector3)moveDirection * 1;
        //Block movingPart = action == Action.grappling ? this : arm;
        //movingPart.transform.position += (Vector3)moveDirection * 1;
        if (action == Action.extending)
            arm.transform.position += (Vector3)moveDirection * 1;

        //add moving blocks back to index
        foreach (Block movingBlock in movingBlocks)
            gridIndex.AddToIndex(movingBlock);
        if (action == Action.extending)
            gridIndex.AddToIndex(arm);

        //if not extending, parent arm
        if (action != Action.extending)
            arm.transform.SetParent(transform);
        //end moving


        //fasten
        if (action == Action.extending)
            ChangeFastening(this, arm, true, false); //fasten arm and body (NO ICON!!!)

        if (action == Action.grappling) //if target is fastened, unfasten arm/target and fasten body/target
        {
            targetBlock = GetTargetBlock();
            if (targetBlock != null)
            {
                string key = GetFastenerKey(arm, targetBlock);
                if (fasteners.ContainsKey(key))
                {
                    ChangeFastening(arm, targetBlock, false);
                    ChangeFastening(this, targetBlock, true);
                }
            }
        }
        else if (targetBlock != null && targetIsFastened) //re-fasten target if it was previously fastened
        {
            Block targetFastenedPart = action == Action.extending ? arm : this;
            ChangeFastening(targetFastenedPart, targetBlock, true);
        }


        sr.sprite = action == Action.extending ? extendedBodySprite : bodySprite;

        if (action == Action.retracting || action == Action.grappling)
            action = Action.extending;
        else //if extending
            action = pistonType == "retractor" ? Action.retracting : Action.grappling;

        Reset();
    }

    private void ChangeFastening(Block block1, Block block2, bool fasten, bool createIcon = true)
    {
        Block[] blocks = OrganizedBlocks(block1, block2); //organize by blockNumber, lowest first
        string key = GetFastenerKey(blocks[0], blocks[1]);

        if (fasten && !fasteners.ContainsKey(key))
            CreateFastener(key, blocks[0], blocks[1], createIcon);
        else if (fasteners.ContainsKey(key)) //unfasten
            DestroyFastener(key);
    }

    private void Reset()
    {
        targetBlock = null;
        targetIsFastened = false;

        movingBlocks.Clear();
        checkingBlocks = 0;
        error = false;
        errorBlock = null;
    }
}