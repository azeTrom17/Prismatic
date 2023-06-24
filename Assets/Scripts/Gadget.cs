using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Gadget : Block, IPointerDownHandler
{
    //assigned in prefab:
    public GameObject clickZone;

    //dynamic
    private Vector2 mousePosition;
    private bool dragging;
    private Vector2 dragOffset;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle) //or if in editor
        {
            dragOffset = mousePosition - (Vector2)clickZone.transform.position;
            dragging = true;
        }
        else
            ActivateGadget((int)eventData.button);
    }

    public abstract void ActivateGadget(int mouseButton);

    protected virtual void Update()
    {
        mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(2)) //or any other mouse button in editor
            dragging = false;

        if (dragging)
            clickZone.transform.position = mousePosition - dragOffset;
    }

    protected Block GetTargetBlock() //helper method for fasteners/pistons/hinges
    {
        Vector2 targetPosition = transform.position + (transform.up * 1);

        return gridIndex.GetBlockFromIndex(targetPosition);
    }
}