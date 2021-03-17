using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_NormController : MonoBehaviour
{
    Rigidbody rb;
    CarParams carParams;
    RoadRules roadRules;
    RaycastHit obstHitFront;
    RaycastHit obstHitSide;

    float maxSpeed_kmh;
    float mobility;
    float frontDetDist;
    float sideDetDist;
    float maxSpeed;
    public float maxAllowedSpeed;
    float xPos;
    float yPos;
    float zPos;
    public int curLane;
    readonly float force = 100000;
    readonly float xBound = 8;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        roadRules = FindObjectOfType<RoadRules>();

        // getting values
        carParams = GetComponent<CarParams>();
        maxSpeed_kmh = carParams.maxSpeed_kmh;
        mobility = carParams.mobility;
        frontDetDist = carParams.frontDetDist;
        sideDetDist = carParams.sideDetDist;

        maxSpeed = maxSpeed_kmh * 1000 / 2600 / 25 * force; // km/h -> m/frame * force
    }

    // Update is called once per frame
    void Update()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;
        maxAllowedSpeed = roadRules.maxAllowedSpeed_kmh[curLane];

        DetectCurLane();
        AvoidObstacles();
        HoldYourLane();
        StayOnRoad();
    }

    void AvoidObstacles()
    {
        if (FrontObstacles())
        {
            MoveForward(0);
            Overtake();

            if (obstHitFront.collider.CompareTag("Traffic Car"))
            {
                
            }
        }
        else
        {
            MoveForward(maxSpeed);
        }
    }

    void MoveForward(float speed)
    {
        float playerSpeed = Input.GetAxis("Vertical") * 1.9f * force;
        rb.AddForce(Vector3.forward * (speed - playerSpeed) * Time.deltaTime, ForceMode.Impulse);

        if (curLane > 0)
        {
            Vector3 rightRay1Pos = new Vector3(xPos + 4, .3f, zPos + transform.localScale.z / 2);
            Ray rightRay1 = new Ray(rightRay1Pos, Vector3.forward);
            Debug.DrawRay(rightRay1Pos, Vector3.forward * frontDetDist * 2);

            Vector3 rightRay2Pos = new Vector3(xPos, .3f, zPos);
            Ray rightRay2 = new Ray(rightRay2Pos, Vector3.right);
            Debug.DrawRay(rightRay2Pos, Vector3.right * sideDetDist);

            Vector3 rightRay3Pos = new Vector3(xPos + 4, .3f, zPos - transform.localScale.z / 2);
            Ray rightRay3 = new Ray(rightRay3Pos, Vector3.back);
            Debug.DrawRay(rightRay3Pos, Vector3.back * sideDetDist * 3);

            if (!Physics.Raycast(rightRay1, out obstHitSide, frontDetDist * 2) &&
                !Physics.Raycast(rightRay2, out obstHitSide, sideDetDist) &&
                !Physics.Raycast(rightRay3, out obstHitSide, sideDetDist * 3))
            {
                MoveToLane(curLane - 1);
            }
        }        
    }

    void DetectCurLane()
    {
        if (xPos >= 8 && xPos < 12)
            curLane = 0;
        else if (xPos >= 4 && xPos < 8)
            curLane = 1;
        else if (xPos >= 0 && xPos < 4)
            curLane = 2;
        else if (xPos >= -4 && xPos < 0)
            curLane = 3;
        else
            curLane = 4;
    }

    void HoldYourLane()
    {
        MoveToLane(curLane);
    }

    void Overtake()
    {
        if (curLane < 3)
        {
            Vector3 leftRay1Pos = new Vector3(xPos - 4, .3f, zPos + transform.localScale.z / 2);
            Ray leftRay1 = new Ray(leftRay1Pos, Vector3.forward);
            Debug.DrawRay(leftRay1Pos, Vector3.forward * sideDetDist * 3);

            Vector3 leftRay2Pos = new Vector3(xPos, .3f, zPos);
            Ray leftRay2 = new Ray(leftRay2Pos, Vector3.left);
            Debug.DrawRay(leftRay2Pos, Vector3.left * sideDetDist);

            Vector3 leftRay3Pos = new Vector3(xPos - 4, .3f, zPos - transform.localScale.z / 2);
            Ray leftRay3 = new Ray(leftRay3Pos, Vector3.back);
            Debug.DrawRay(leftRay3Pos, Vector3.back * sideDetDist * 2);

            if (!Physics.Raycast(leftRay1, sideDetDist * 3) &&
                !Physics.Raycast(leftRay2, sideDetDist) &&
                !Physics.Raycast(leftRay3, sideDetDist * 2))
            {
                MoveToLane(curLane + 1);
            }
        }
    }

    void MoveToLane(int laneIndex)
    {
        float laneMiddle = FindObjectOfType<RoadRules>().laneMiddleX[laneIndex];
        float xDirection = laneMiddle - xPos;
        rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
    }

    bool FrontObstacles()
    {
        // Front view 1
        Vector3 frontRay1Pos = new Vector3(xPos, 1, zPos);
        Ray frontRay1 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay1Pos, Vector3.forward * frontDetDist);

        // Front view 2
        Vector3 frontRay2Pos = new Vector3(xPos - 1.5f, 1, zPos);
        Ray frontRay2 = new Ray(frontRay2Pos, Vector3.forward);
        Debug.DrawRay(frontRay2Pos, Vector3.forward * frontDetDist);

        // Front view 3
        Vector3 frontRay3Pos = new Vector3(xPos + 1.5f, 1, zPos);
        Ray frontRay3 = new Ray(frontRay3Pos, Vector3.forward);
        Debug.DrawRay(frontRay3Pos, Vector3.forward * frontDetDist);

        return Physics.Raycast(frontRay1, out obstHitFront, frontDetDist) &&
            Physics.Raycast(frontRay2, out obstHitFront, frontDetDist) &&
            Physics.Raycast(frontRay3, out obstHitFront, frontDetDist);
    }
    bool LeftObctacles()
    {
        // Front Left View
        Vector3 frontLeftViewPos = new Vector3(xPos - 4, 1, zPos);
        Ray frontLeftView = new Ray(frontLeftViewPos, Vector3.forward);
        Debug.DrawRay(frontLeftViewPos, Vector3.forward * sideDetDist);

        // Left View
        Vector3 leftViewPos = new Vector3(xPos, 1, zPos);
        Ray leftView = new Ray(leftViewPos, Vector3.right);
        Debug.DrawRay(leftViewPos, Vector3.left * sideDetDist);

        // Left Mirror
        Vector3 leftMirrorPos = new Vector3(xPos - 4, 1, zPos + 1);
        Ray leftMirror = new Ray(leftMirrorPos, Vector3.forward);
        Debug.DrawRay(leftMirrorPos, Vector3.back * sideDetDist * 2);

        return Physics.Raycast(frontLeftView, sideDetDist) &&
            Physics.Raycast(leftView, sideDetDist) &&
            Physics.Raycast(leftMirror, sideDetDist * 2);
    }
    bool RightObstacles()
    {
        // Front Right
        Vector3 frontRightViewPos = new Vector3(xPos + 4, 1, zPos);
        Ray frontRightView = new Ray(frontRightViewPos, Vector3.forward);
        Debug.DrawRay(frontRightViewPos, Vector3.forward * frontDetDist * 2);

        // Right
        Vector3 rightViewPos = new Vector3(xPos, 1, zPos);
        Ray rightView = new Ray(rightViewPos, Vector3.right);
        Debug.DrawRay(rightViewPos, Vector3.right * sideDetDist);

        // Rear Right
        Vector3 rightMirrorPos = new Vector3(xPos + 4, 1, zPos + 1);
        Ray rightMirror = new Ray(rightMirrorPos, Vector3.forward);
        Debug.DrawRay(rightMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(frontRightView, frontDetDist * 2) &&
            Physics.Raycast(rightView, sideDetDist) &&
            Physics.Raycast(rightMirror, sideDetDist * 3);
    }
    bool RearObstacles()
    {
        Vector3 centerMirrorPos = new Vector3(xPos, 1.4f, zPos + 1);
        Ray centerMirror = new Ray(centerMirrorPos, Vector3.forward);
        Debug.DrawRay(centerMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(centerMirror, out obstHitFront, frontDetDist);
    }

    void StayOnRoad()
    {
        if (xPos < -xBound)
            transform.position = new Vector3(-xBound, yPos, zPos);
        if (xPos > 10)
            transform.position = new Vector3(10, yPos, zPos);
    }
}
