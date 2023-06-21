using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Gadget
{
    //assigned in prefab
    public string pistonType; //"retractor" or "grappler"

    //dynamic
    private int stroke; //1 or 2. Stroke 1 extends, 2 retracts/grappples

    public override void ActivateGadget(int mouseButton)
    {

    }

    //error if arm is fastened to the body elsewhere
}