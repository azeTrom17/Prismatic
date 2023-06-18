using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldBlock : MonoBehaviour
{
    public Transform spinTr;
    public Transform anchorTr;
    public Rigidbody2D spinRb;
    public Rigidbody2D anchorRb;

    private readonly float rotateDuration = .5f;
    private bool rotating;
    private float rotateTimeElapsed = 0;

    private void Start()
    {
        StartCoroutine(Test());
    }
    private IEnumerator Test()
    {
        yield return new WaitForSeconds(1);
        //if I comment THIS LINE^, it works perfectly

        //spinRb.mass = 1000;
        rotateTimeElapsed = 0;
        rotating = true;

        yield return new WaitForSeconds(rotateDuration);

        rotating = false;
        spinRb.mass = 1; //default
        spinRb.angularVelocity = 0;

        Quaternion newRotation = Quaternion.Euler(0, 0, SnapRotation(spinTr.eulerAngles.z));
        //position should never change, but just in case
        spinRb.transform.SetPositionAndRotation(anchorTr.position + 1 * -anchorTr.right, newRotation);

        yield return new WaitForFixedUpdate();
        FixedJoint2D newJoint = anchorRb.gameObject.AddComponent<FixedJoint2D>();
        newJoint.connectedBody = spinRb;
    }
    private void FixedUpdate()
    {
        if (rotating)
        {
            float angle = Mathf.Lerp(0, 90, rotateTimeElapsed / rotateDuration);
            spinRb.MoveRotation(angle);

            rotateTimeElapsed += Time.fixedDeltaTime;
        }
    }

    private float SnapRotation(float zRotation)
    {
        float snapAngle = anchorTr.rotation.eulerAngles.z;

        //checks 0, 90, 180, 270, 360
        for (int i = 0; i < 5; i++)
        {
            //check both positive and negative angles
            if (Mathf.Abs(zRotation - snapAngle) < 20)
                return snapAngle;
            if (Mathf.Abs(zRotation + snapAngle) < 20)
                return -snapAngle;

            snapAngle += 90;
        }
        return zRotation;
    }
}