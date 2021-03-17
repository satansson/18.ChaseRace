using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_SlowController : MonoBehaviour
{
    Rigidbody rb;
    CarParams carParams;
    RoadRules roadRules;
    RaycastHit obstacleHit;

    float maxSpeed_kmh;
    float mobility;
    float frontDetDist;
    float sideDetDist;
    float maxSpeed;
    float maxAllowedSpeed;
    float xPos;
    float yPos;
    float zPos;
    int curLane;
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
        maxAllowedSpeed = roadRules.maxAllowedSpeed_kmh[curLane] + 9;

        DetectCurLane();
        StayOnRoad();
        MoveOnSpeed(maxAllowedSpeed);

        if (!FrontObstacles())
        {

        }
        else if (obstacleHit.collider.CompareTag("Traffic Car") && curLane < 4) // Overtake traffic obctacles
        {
            MoveOnSpeed(0);
            ChangeLane(curLane + 1);
        }
        else
        {
            MoveOnSpeed(0);
        }

        if (RearView() && curLane == 4)
        {
            ChangeLane(3);
        }


        HoldYourLane();

    }

    void MoveOnSpeed(float speed)
    {
        float playerSpeed = Input.GetAxis("Vertical") * 1.9f * force;
        rb.AddForce(Vector3.forward * (speed - playerSpeed) * Time.deltaTime, ForceMode.Impulse);
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
    void HoldYourLane() // ??
    {
        ChangeLane(curLane);
    }
    void StayOnRoad()
    {
        if (xPos < -xBound)
            transform.position = new Vector3(-xBound, yPos, zPos);
        if (xPos > 10)
            transform.position = new Vector3(10, yPos, zPos);
    }
    void ChangeLane(int laneIndex)
    {
        float laneMiddle = FindObjectOfType<RoadRules>().laneMiddleX[laneIndex];
        float xDirection = laneMiddle - xPos;

        if (xDirection > 0 && !RightObstacles()) // if move right
        {
            rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
        }
        else if (xDirection < 0 && !LeftObctacles()) // if move left
        {
            rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
        }
    }

    bool FrontObstacles()
    {
        Vector3 frontRay1Pos = new Vector3(xPos, 1, zPos);
        Ray frontRay1 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay1Pos, Vector3.forward * frontDetDist);

        Vector3 frontRay2Pos = new Vector3(xPos - 1.5f, 1, zPos);
        Ray frontRay2 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay2Pos, Vector3.forward * frontDetDist);

        Vector3 frontRay3Pos = new Vector3(xPos + 1.5f, 1, zPos);
        Ray frontRay3 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay3Pos, Vector3.forward * frontDetDist);

        return Physics.Raycast(frontRay1, out obstacleHit, frontDetDist) &&
            Physics.Raycast(frontRay2, out obstacleHit, frontDetDist) &&
            Physics.Raycast(frontRay3, out obstacleHit, frontDetDist);
    }
    bool LeftObctacles()
    {
        // Front Left
        Vector3 frontLeftViewPos = new Vector3(xPos - 4, 1, zPos);
        Ray frontLeftView = new Ray(frontLeftViewPos, Vector3.forward);
        Debug.DrawRay(frontLeftViewPos, Vector3.forward * sideDetDist);

        // Left
        Vector3 leftViewPos = new Vector3(xPos, 1, zPos);
        Ray leftView = new Ray(leftViewPos, Vector3.right);
        Debug.DrawRay(leftViewPos, Vector3.left * sideDetDist);

        // Rear Left
        Vector3 leftMirrorPos = new Vector3(xPos - 4, 1, zPos + 1);
        Ray leftMirror = new Ray(leftMirrorPos, Vector3.forward);
        Debug.DrawRay(leftMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(frontLeftView, sideDetDist) &&
            Physics.Raycast(leftView, sideDetDist) &&
            Physics.Raycast(leftMirror, sideDetDist * 3);
    }
    bool RightObstacles()
    {
        // Front Right
        Vector3 frontRightViewPos = new Vector3(xPos + 4, 1, zPos);
        Ray frontRightView = new Ray(frontRightViewPos, Vector3.forward);
        Debug.DrawRay(frontRightViewPos, Vector3.forward * frontDetDist);

        // Right
        Vector3 rightViewPos = new Vector3(xPos, 1, zPos);
        Ray rightView = new Ray(rightViewPos, Vector3.right);
        Debug.DrawRay(rightViewPos, Vector3.right * sideDetDist);

        // Rear Right
        Vector3 rightMirrorPos = new Vector3(xPos + 4, 1, zPos + 1);
        Ray rightMirror = new Ray(rightMirrorPos, Vector3.forward);
        Debug.DrawRay(rightMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(frontRightView, frontDetDist) &&
            Physics.Raycast(rightView, sideDetDist) &&
            Physics.Raycast(rightMirror, sideDetDist * 3);
    }
    bool RearView()
    {
        Vector3 centerMirrorPos = new Vector3(xPos, 1.4f, zPos + 1);
        Ray centerMirror = new Ray(centerMirrorPos, Vector3.forward);
        Debug.DrawRay(centerMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(centerMirror, out obstacleHit, frontDetDist);
    }


}
