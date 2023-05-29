using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    //assigned in prefab
    public GameObject body;
    public Transform bodyTr;
    public Rigidbody2D bodyRb;
    public BoxCollider2D bodyCol;
    public Transform armTr;
    public Rigidbody2D armRb;
    public BoxCollider2D armCol;

    private FixedJoint2D currentArmBodyJoint;

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
        LinkBodyAndArm(true);
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
    private void LinkBodyAndArm(bool link)
    {
        if (link)
        {
            currentArmBodyJoint = body.AddComponent<FixedJoint2D>();
            currentArmBodyJoint.connectedBody = armRb;
        }
        else
            Destroy(currentArmBodyJoint);
    }

    //main methods:
    private IEnumerator Extend()
    {
        LinkBodyAndArm(false);
        armRb.mass = 1000;
        extendTimeElapsed = 0;
        extending = true;

        yield return new WaitForSeconds(extendDuration);

        extending = false;
        armRb.mass = .5f; //default value
        armTr.SetPositionAndRotation(bodyTr.position + (1 * bodyTr.up), bodyTr.rotation);
        LinkBodyAndArm(true);
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
        LinkBodyAndArm(false);
        armRb.mass = 1000;
        retractTimeElapsed = 0;
        retracting = true;

        yield return new WaitForSeconds(retractDuration);

        retracting = false;
        armRb.mass = .5f; //default value
        armTr.SetPositionAndRotation(bodyTr.position, bodyTr.rotation);
        LinkBodyAndArm(true);
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
        LinkBodyAndArm(false);
        bodyRb.mass = 1000;
        grappleTimeElapsed = 0;
        grappling = true;

        yield return new WaitForSeconds(grappleDuration);

        grappling = false;
        bodyRb.mass = .5f; //default value
        bodyTr.SetPositionAndRotation(armTr.position, armTr.rotation);
        LinkBodyAndArm(true);
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