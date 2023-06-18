using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hinge : MonoBehaviour
{
    //assigned in prefab
    public Transform bodyTr;
    public Rigidbody2D bodyRb;
    public BoxCollider2D bodyCol;
    public FixedJoint2D initialJoint;
    public GameObject arm;
    public Transform armTr;
    public Rigidbody2D armRb;
    public BoxCollider2D armCol;

    //dynamic
    //initially true if right hinge, false if left
    private bool clockwise = false;
    private Joint2D currentJoint;

    //rotating
    private readonly float rotateDuration = .5f;
    private bool rotating;
    private float rotateTimeElapsed = 0;
    private float initialRotation; //set when rotation begins

    //basic methods:
    private void Awake()
    {
        Physics2D.IgnoreCollision(bodyCol, armCol);
        currentJoint = initialJoint;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            StartCoroutine(Rotate());
    }

    private void FixedUpdate()
    {
        RotateUpdate();
    }

    //helper methods:
    private void SwitchJoints(bool unlock)
    {
        Joint2D newJoint;

        if (unlock)
        {
            HingeJoint2D hinge = arm.AddComponent<HingeJoint2D>();
            hinge.autoConfigureConnectedAnchor = false;
            newJoint = hinge;
        }
        else
        {
            FixedJoint2D fixedJoint = arm.AddComponent<FixedJoint2D>();
            fixedJoint.autoConfigureConnectedAnchor = false;
            newJoint = fixedJoint;
        }

        newJoint.connectedBody = bodyRb;
        Destroy(currentJoint);
        currentJoint = newJoint;
    }

    //main methods:
    private IEnumerator Rotate()
    {
        SwitchJoints(true);
        bodyRb.mass = 1000;
        armRb.mass = 1000;
        initialRotation = armTr.eulerAngles.z;
        rotateTimeElapsed = 0;
        rotating = true;

        yield return new WaitForSeconds(rotateDuration);

        rotating = false;
        bodyRb.mass = 1; //default value
        armRb.mass = 1; //^
        armRb.angularVelocity = 0;
        Quaternion newRotation = Quaternion.Euler(0, 0, SnapRotation(armTr.eulerAngles.z));
        //position should never change, but just in case
        armTr.SetPositionAndRotation(bodyTr.position + (1 * -bodyTr.right), newRotation);
        armRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        bodyRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //yield return new WaitForFixedUpdate(); //prevents bug, can't create joint as rotation is ending
        SwitchJoints(false);
        //yield return new WaitForFixedUpdate(); //prevents bug, can't create joint as rotation is ending
        armRb.constraints = RigidbodyConstraints2D.None;
        bodyRb.constraints = RigidbodyConstraints2D.None;

        clockwise = !clockwise; //reverse next rotation
    }
    private void RotateUpdate() //run in fixedupdate
    {
        if (!rotating) return;

        float direction = clockwise ? -1 : 1;
        float angle = Mathf.Lerp(0, 90 * direction, rotateTimeElapsed / rotateDuration);
        armRb.MoveRotation(angle + initialRotation);

        rotateTimeElapsed += Time.fixedDeltaTime;
    }
    private float SnapRotation(float zRotation)
    {
        float snapAngle = bodyTr.rotation.eulerAngles.z;

        //checks 0, 90, 180, 270, 360
        for (int i = 0; i < 5; i++)
        {
            //check both positive and negative angles
            if (Mathf.Abs(zRotation - snapAngle) < 10)
                return snapAngle;
            if (Mathf.Abs(zRotation + snapAngle) < 10)
                return -snapAngle;

            snapAngle -= 90;
        }
        return zRotation;
    }
}