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

    private void LeftActivation() //left click
    {
        Block targetBlock = GetTargetBlock();
        if (targetBlock == null) return;

        Block[] blocks = OrganizedBlocks(this, targetBlock); //organize by blockNumber, lowest first

        string key = GetFastenerKey(blocks[0], blocks[1]);

        if (fasteners.ContainsKey(key))
            DestroyFastener(key);
        else
            CreateFastener(key, blocks[0], blocks[1], true);
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