using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPiston : MonoBehaviour
{
    //assigned in prefab
    public GameObject body;
    public Transform bodyTr;
    public Rigidbody2D bodyRb;
    public BoxCollider2D bodyCol;
    public FixedJoint2D initialJoint;
    public Transform armTr;
    public Rigidbody2D armRb;
    public BoxCollider2D armCol;

    //dynamic
    private Joint2D currentJoint;

    //extending
    private readonly float extendDuration = .125f;
    private bool extending;
    private float extendTimeElapsed;

    //retracting
    private readonly float retractDuration = .125f;
    private bool retracting;
    private float retractTimeElapsed;

    //grappling
    private readonly float grappleDuration = .125f;
    private bool grappling;
    private float grappleTimeElapsed;

    //basic methods:
    private void Awake()
    {
        Physics2D.IgnoreCollision(bodyCol, armCol);
        currentJoint = initialJoint;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(Extend());
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Retract());
        if (Input.GetKeyDown(KeyCode.G))
            StartCoroutine(Grapple());
    }

    private void FixedUpdate()
    {
        ExtendUpdate();
        RetractUpdate();
        GrappleUpdate();
    }

    //helper methods:
    private void SwitchJoints(bool unlock)
    {
        Joint2D newJoint;

        if (unlock)
        {
            SliderJoint2D slider = body.AddComponent<SliderJoint2D>();
            slider.autoConfigureAngle = false;
            slider.angle = 90;
            newJoint = slider;
        }
        else
            newJoint = body.AddComponent<FixedJoint2D>();

        newJoint.connectedBody = armRb;
        Destroy(currentJoint);
        currentJoint = newJoint;
    }

    //main methods:
    private IEnumerator Extend()
    {
        SwitchJoints(true);
        armRb.mass = 1000;
        extendTimeElapsed = 0;
        extending = true;

        yield return new WaitForSeconds(extendDuration);

        extending = false;
        armRb.mass = 1; //more mass when extended
        bodyRb.mass = 1; //^
        armTr.SetPositionAndRotation(bodyTr.position + (1 * bodyTr.up), bodyTr.rotation);
        SwitchJoints(false);
    }
    private void ExtendUpdate() //run in fixedupdate
    {
        if (!extending) return;

        float yValue = Mathf.Lerp(0, 1, extendTimeElapsed / extendDuration);
        Vector2 position = bodyTr.position + (yValue * bodyTr.up);
        armRb.MovePosition(position);

        extendTimeElapsed += Time.fixedDeltaTime;
    }

    private IEnumerator Retract()
    {
        SwitchJoints(true);
        armRb.mass = 1000;
        retractTimeElapsed = 0;
        retracting = true;

        yield return new WaitForSeconds(retractDuration);

        retracting = false;
        armRb.mass = .5f; //less mass when not extended
        bodyRb.mass = .5f; //^
        armTr.SetPositionAndRotation(bodyTr.position, bodyTr.rotation);
        SwitchJoints(false);
    }
    private void RetractUpdate() //run in fixedupdate
    {
        if (!retracting) return;

        float yValue = Mathf.Lerp(1, 0, retractTimeElapsed / retractDuration);
        Vector2 position = bodyTr.position + (yValue * bodyTr.up);
        armRb.MovePosition(position);

        retractTimeElapsed += Time.fixedDeltaTime;
    }

    private IEnumerator Grapple()
    {
        SwitchJoints(true);
        bodyRb.mass = 1000;
        grappleTimeElapsed = 0;
        grappling = true;

        yield return new WaitForSeconds(grappleDuration);

        grappling = false;
        armRb.mass = .5f; //less mass when not extended
        bodyRb.mass = .5f; //^
        bodyTr.SetPositionAndRotation(armTr.position, armTr.rotation);
        SwitchJoints(false);
    }
    private void GrappleUpdate() //run in fixedupdate
    {
        if (!grappling) return;

        float yValue = Mathf.Lerp(-1, 0, grappleTimeElapsed / grappleDuration);
        Vector2 position = armTr.position + (yValue * armTr.up);
        bodyRb.MovePosition(position);

        grappleTimeElapsed += Time.fixedDeltaTime;
    }
}