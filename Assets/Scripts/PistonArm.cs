using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonArm : Block
{
    protected override void Start()
    {
        //don't use base.Start; can't add to index since body is in the same position

        blockNumber = nextAvailableBlockNumber;
        nextAvailableBlockNumber++;
    }
}