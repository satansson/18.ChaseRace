using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Car_FastController : MonoBehaviour
{
    Rigidbody rb;
    CarParams carParams;
    RoadRules roadRules;
    RaycastHit obstHit;
    [SerializeField] GameObject centerOfMass;
    [SerializeField] TextMeshProUGUI speed;

    float maxSpeed_kmh;
    float mobility;
    float frontDetDist;
    float sideDetDist;
    float maxSpeed;
    float maxAllowedSpeed;
    float xPos;
    float yPos;
    float zPos;
    [SerializeField] int curLane;
    [SerializeField] float curSpeed;
    readonly float force = 100000;
    readonly float xBound = 8;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        roadRules = GetComponent<RoadRules>();

        // getting values
        carParams = GetComponent<CarParams>();
        maxSpeed_kmh = carParams.maxSpeed_kmh;
        mobility = carParams.mobility;
        frontDetDist = carParams.frontDetDist;
        sideDetDist = carParams.sideDetDist;
        //rb.centerOfMass = centerOfMass.transform.position;

        maxSpeed = maxSpeed_kmh * 1000 / 3600 / 25 * force; // km/h -> m/frame * force
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: 1. Car doesn't see the obctacles excluding sometimes buses.
        // TODO: 2. ChooseLaneLogic


        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;
        maxAllowedSpeed = roadRules.maxAllowedSpeed_kmh[curLane] * 1000 / 3600 / 25 * force; // km/h -> m/frame * force


        if (FrontObstacles())
        {
            if (obstHit.collider.CompareTag("CopCar")) // Stop at CopCar.
            {
                Debug.Log("A cop car is an obstacle!");
            }
            else if (obstHit.collider.CompareTag("Bus")) // Overtake buses...
            {
                Debug.Log("A bus is an obstacle!");
                Overtake();
            }
            else if (obstHit.collider.CompareTag("Traffic Car")) // ...and traffic cars
            {
                Debug.Log("A traffic car is an obstacle!");
                Overtake();
            }
            else if (obstHit.collider.CompareTag("Motorbike")) // ...and traffic cars
            {
                Debug.Log("A motorbike is an obstacle!");
                Overtake();
            }
            else
            {
                Debug.Log("An unknown obstacle!");
                Overtake();
            }
        }
        else
        {
            if (maxSpeed < maxAllowedSpeed) // Reduce maxAllowedSpeed to maxSpeed
            {
                Debug.Log("Max Available Speed!");
                MoveForward(maxSpeed);
            }
            else
            {
                Debug.Log("Max Allowed Speed!");
                MoveForward(maxAllowedSpeed);
            }
        }

        curSpeed = Mathf.RoundToInt(rb.velocity.magnitude * 3.6f);

        //ChooseRightLane();
        //HoldYourLane();


        DetectCurLane();
        StayOnRoad();


    }


    void ChooseRightLane()
    {
        if (curLane > 1)
        {
            MoveToLane(curLane - 1);
        }

        if (maxSpeed > maxAllowedSpeed + 10 && curLane < 4)
            MoveToLane(curLane + 1);
    }

    void HoldYourLane() // isNeeded?
    {
        MoveToLane(curLane);
    }

    void Overtake()
    {
        if (curLane < 4 && !LeftObctacles())
        {
            Debug.Log("Trying to overtake on the left...");
            MoveToLane(curLane + 1);
        }
        else
        {
            Debug.Log("Trying to overtake on the right...");
            MoveToLane(curLane - 1);
        }
    }

    void MoveToLane(int laneIndex) // Checking side obstacles
    {
        float laneMiddle = FindObjectOfType<RoadRules>().laneMiddleX[laneIndex];
        float xDirection = laneMiddle - xPos;

        if (xDirection > 0 && !RightObstacles()) // if move right
        {
            Debug.Log("Start moving left...");
            rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
        }
        else if (xDirection < 0 && !LeftObctacles()) // if move left
        {
            Debug.Log("Start moving right...");
            rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
        }
    }

    //void MoveToLane(int laneIndex)
    //{
    //    float laneMiddle = FindObjectOfType<RoadRules>().laneMiddleX[laneIndex];
    //    float xDirection = laneMiddle - xPos;
    //    rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
    //}

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

        return Physics.Raycast(frontRay1, out obstHit, frontDetDist) &&
            Physics.Raycast(frontRay2, out obstHit, frontDetDist) &&
            Physics.Raycast(frontRay3, out obstHit, frontDetDist);


        //// Front view 1
        //Vector3 frontRay1Pos = new Vector3(xPos, 1, zPos);
        //Ray frontRay1 = new Ray(frontRay1Pos, Vector3.forward);
        //Debug.DrawRay(frontRay1Pos, Vector3.forward * frontDetDist);

        //// Front view 2
        //Vector3 frontRay2Pos = new Vector3(xPos - .8f, 1, zPos);
        //Ray frontRay2 = new Ray(frontRay2Pos, Vector3.forward);
        //Debug.DrawRay(frontRay2Pos, Vector3.forward * frontDetDist);

        //// Front view 3
        //Vector3 frontRay3Pos = new Vector3(xPos + .8f, 1, zPos);
        //Ray frontRay3 = new Ray(frontRay3Pos, Vector3.forward);
        //Debug.DrawRay(frontRay3Pos, Vector3.forward * frontDetDist);

        //// Front view 4
        //Vector3 frontRay4Pos = new Vector3(xPos - 1.6f, 1, zPos);
        //Ray frontRay4 = new Ray(frontRay4Pos, Vector3.forward);
        //Debug.DrawRay(frontRay4Pos, Vector3.forward * frontDetDist * .75f);

        //// Front view 5
        //Vector3 frontRay5Pos = new Vector3(xPos + 1.6f, 1, zPos);
        //Ray frontRay5 = new Ray(frontRay5Pos, Vector3.forward);
        //Debug.DrawRay(frontRay5Pos, Vector3.forward * frontDetDist * .75f);

        //return
        //    Physics.Raycast(frontRay1, out obstHit, frontDetDist) &&
        //    Physics.Raycast(frontRay2, out obstHit, frontDetDist) &&
        //    Physics.Raycast(frontRay3, out obstHit, frontDetDist) &&
        //    Physics.Raycast(frontRay4, out obstHit, frontDetDist * .75f) &&
        //    Physics.Raycast(frontRay5, out obstHit, frontDetDist * .75f);
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
        Debug.DrawRay(frontRightViewPos, Vector3.forward * frontDetDist);

        // Right
        Vector3 rightViewPos = new Vector3(xPos, 1, zPos);
        Ray rightView = new Ray(rightViewPos, Vector3.right);
        Debug.DrawRay(rightViewPos, Vector3.right * sideDetDist);

        // Rear Right
        Vector3 rightMirrorPos = new Vector3(xPos + 4, 1, zPos + 1);
        Ray rightMirror = new Ray(rightMirrorPos, Vector3.forward);
        Debug.DrawRay(rightMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(frontRightView, frontDetDist) && Physics.Raycast(rightView, sideDetDist) && Physics.Raycast(rightMirror, sideDetDist * 3);
    }

    bool RearObstacles() // move to ChooseRightLane()
    {
        Vector3 centerMirrorPos = new Vector3(xPos, 1.4f, zPos + 1);
        Ray centerMirror = new Ray(centerMirrorPos, Vector3.forward);
        Debug.DrawRay(centerMirrorPos, Vector3.back * sideDetDist * 3);

        return Physics.Raycast(centerMirror, out obstHit, frontDetDist);
    }



    void MoveForward(float speed)
    {
        float playerSpeed = Input.GetAxis("Vertical") * 1.9f * force;
        rb.AddRelativeForce(Vector3.forward * (speed - playerSpeed));
    } // +
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
    } // +
    void StayOnRoad()
    {
        if (xPos < -xBound)
            transform.position = new Vector3(-xBound, yPos, zPos);
        if (xPos > 11)
            transform.position = new Vector3(10, yPos, zPos);
    } // +

}
