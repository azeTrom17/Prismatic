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

        //if target is the side of a piston arm, (and not the end) return
        if (targetBlock.CompareTag("PistonArm"))
        {
            Vector2 position = targetBlock.transform.position + targetBlock.transform.up * 1;
            if (gridIndex.GetBlockFromIndex(position) != this)
                return;
        }

        string key = GetFastenerKey(this, targetBlock);
        if (fasteners.ContainsKey(key))
            DestroyFastener(key);
        else
            CreateFastener(key, this, targetBlock, true);
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