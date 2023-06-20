using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hinge : Gadget
{
    //assigned in prefab
    public string hingeType; //"clockwise" or "counterclockwise"

    //dynamic
    private int stroke; //1 or 2. Stroke 1 matches hingeType, 2 is opposite

    public override void ActivateGadget(int mouseButton)
    {

    }


    //error if target is fastened to the hinge elsewhere
}