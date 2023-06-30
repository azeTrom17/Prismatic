using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Gadget
{
    //ADD BOMB RIGHT CLICK

    private readonly List<Block> blocksInRange = new();
    public override void ActivateGadget(int mouseButton)
    {
        blocksInRange.Add(this);

        Vector2 up = transform.up;
        Vector2 right = transform.right;

        AddBlockInRange(up * 1);
        AddBlockInRange(up * 2);
        AddBlockInRange(up * -1);
        AddBlockInRange(up * -2);

        AddBlockInRange(right * 1);
        AddBlockInRange(right * 2);
        AddBlockInRange(right * -1);
        AddBlockInRange(right * -2);

        AddBlockInRange(up * 1 + right * 1);
        AddBlockInRange(up * 2 + right * 1);
        AddBlockInRange(up * 1 + right * 2);

        AddBlockInRange(up * -1 + right * 1);
        AddBlockInRange(up * -2 + right * 1);
        AddBlockInRange(up * -1 + right * 2);

        AddBlockInRange(up * 1 + right * -1);
        AddBlockInRange(up * 2 + right * -1);
        AddBlockInRange(up * 1 + right * -2);

        AddBlockInRange(up * -1 + right * -1);
        AddBlockInRange(up * -2 + right * -1);
        AddBlockInRange(up * -1 + right * -2);

        foreach (Block block in blocksInRange)
            block.DestroyBlock();
    }

    private void AddBlockInRange(Vector2 newOffset)
    {
        newOffset += (Vector2)transform.position;
        Block newBlock = gridIndex.GetBlockFromIndex(newOffset);
        if (newBlock != null)
            blocksInRange.Add(newBlock);
    }
}