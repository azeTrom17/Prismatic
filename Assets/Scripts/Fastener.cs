using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fastener : Gadget
{
    public override void ActivateGadget(int mouseButton)
    {
        Vector2 targetPosition = transform.position + transform.up * 1;
        Block targetBlock = gridIndex.GetBlockFromIndex(targetPosition);

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
}