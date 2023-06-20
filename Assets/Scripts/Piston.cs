using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Gadget
{
    //assigned in prefab
    public string pistonType; //"retractor" or "grappler"

    public override void ActivateGadget(int mouseButton)
    {

    }

    //error if arm is fastened to the body elsewhere
}