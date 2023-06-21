using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fastener : Gadget
{
    private bool waitingForTarget;

    public override void ActivateGadget(int mouseButton)
    {
        if (mouseButton == 0)
            LeftActivation();
        else if (mouseButton == 1)
            RightActivation();
    }

    private Block GetTargetBlock()
    {
        Vector2 targetPosition = transform.position + transform.up * 1;
        return gridIndex.GetBlockFromIndex(targetPosition);
    }

    private void LeftActivation() //left click
    {
        Block targetBlock = GetTargetBlock();
        if (targetBlock == null) return;

        Block[] blocks = OrganizedBlocks(this, targetBlock);
        Block higherBlock = blocks[0]; //higherBlock = block with higher blockNuumber
        Block lowerBlock = blocks[1];

        string key = GetFastenerKey(higherBlock, lowerBlock);

        if (fasteners.ContainsKey(key))
            DestroyFastener(key);
        else
            CreateFastener(key, higherBlock, lowerBlock);
    }

    private void RightActivation() //right click
    {
        Block block = GetTargetBlock();
        if (block != null)
            LeftActivation();
        else
            waitingForTarget = true;
    }

    protected override void Update()
    {
        base.Update();

        if (!waitingForTarget) return;

        Block block = GetTargetBlock();
        if (block == null) return;

        waitingForTarget = false;
        LeftActivation();
    }
}