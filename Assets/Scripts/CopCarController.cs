using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopCarController : MonoBehaviour
{
    public float maxSpeed_kmh;
    public float mobility;
    public float blockDistance;
    public float startOvertaking;
    public float leftOvertakingXLimit;
    public float frontDetDist;
    public float sideDetDist;

    private float force = 100000;
    private float maxSpeed;
    private float normSpeed;
    private float curSpeed;
    private float rearSpeed;
    private float playerSpeed;
    private readonly float xBound = 8;
    private float xPos;
    private float yPos;
    private float zPos;
    private float blockPointX;
    private float blockPointZ;
    private float overtakingPoint;
    private string state;
    private int curLane;
    private bool isChasing;
    private Rigidbody rb;
    private GameObject player;
    private RaycastHit frontHit;
    private RaycastHit leftHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        maxSpeed = maxSpeed_kmh * 1000 / 3600 / 25 * force; // km/h -> m/frame * force
        rearSpeed = -maxSpeed / 2;
        
        //state = "Idle";
        
    }

    void Update()
    {
        normSpeed = FindObjectOfType<RoadRules>().maxAllowedSpeed_kmh[curLane];
        playerSpeed = Input.GetAxis("Vertical") * 1.9f * force; // 240km/h -> m/f * force
        blockPointZ = player.transform.position.z + blockDistance;
        blockPointX = player.transform.position.x - xPos;
        overtakingPoint = player.transform.position.z - startOvertaking;
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;

        Behavior();

        Vector3 sideRayPos = new Vector3(xPos, 0.1f, zPos);
        Ray leftRay = new Ray(sideRayPos, Vector3.forward);
        Debug.DrawRay(sideRayPos, Vector3.forward * sideDetDist);

        if (Physics.Raycast(leftRay, out frontHit, frontDetDist))
        {

        }

        
        {

            DetectCurLane();
            StayOnRoad();
            MoveForward(maxSpeed);
            ChaseTheObject();
        }
    }

    void Behavior()
    {
        Invoke(state + "()", 0);
    }

    void Idle()
    {
        isChasing = false;

        if (curLane == 0)
        {
            MoveForward(0);
        }
        else
        {
            MoveForward(normSpeed);
            MoveToLane(0);
        }
    }

    void MoveToBase()
    {
        isChasing = false;

        if (curLane == 0)
        {
            MoveForward(0);
        }
        else
        {
            MoveForward(normSpeed);
            MoveToLane(0);
        }


    }

    void ChaseTheObject()
    {
        isChasing = true;

        // Moving forvard to the player blocking point...
        if (zPos < blockPointZ)
        {
            MoveForward(maxSpeed);
            // ...on the fastest lane...
            if (zPos < overtakingPoint)
            {
                MoveToLane(3);
            }
            else // ...and overtaking player...
            {
                Overtake();
            }
        }
        else if (zPos > blockPointZ)
        {
            rb.AddForce(Vector3.right * blockPointX * mobility * force * Time.deltaTime, ForceMode.Impulse); // block player
            MoveForward(rearSpeed);
        }
        else
        {
            rb.AddForce(Vector3.right * blockPointX * mobility * force * Time.deltaTime, ForceMode.Impulse); // block player
        }
    }



    void MoveForward(float speed)
    {
        rb.AddForce(Vector3.forward * (speed - playerSpeed) * Time.deltaTime, ForceMode.Impulse);
        AvoidObstacles();
    }

    void AvoidObstacles()
    {
        Vector3 frontRay1Pos = new Vector3(xPos, 0.1f, zPos);
        Ray frontRay1 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay1Pos, Vector3.forward * frontDetDist);

        Vector3 frontRay2Pos = new Vector3(xPos - 1, 0.1f, zPos);
        Ray frontRay2 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay2Pos, Vector3.forward * frontDetDist);

        Vector3 frontRay3Pos = new Vector3(xPos + 1, 0.1f, zPos);
        Ray frontRay3 = new Ray(frontRay1Pos, Vector3.forward);
        Debug.DrawRay(frontRay3Pos, Vector3.forward * frontDetDist);

        if (Physics.Raycast(frontRay1, out frontHit, frontDetDist) &&
            Physics.Raycast(frontRay2, out frontHit, frontDetDist) &&
            Physics.Raycast(frontRay3, out frontHit, frontDetDist))
        {
            if (frontHit.collider.CompareTag("Traffic Car"))
            {
                MoveForward(0);
            }
        }
        else
        {
            MoveForward(maxSpeed);
        }
    }

    void Overtake()
    {
        float leftOvertakingXPos = blockPointX - 3;
        float rightOvertakingXPos = blockPointX + 3f;

        if (player.transform.position.x > leftOvertakingXLimit) // Left overtaking
        {
            rb.AddForce(Vector3.right * leftOvertakingXPos * mobility * force * Time.deltaTime, ForceMode.Impulse);
        }
        else // Right overtaking
        {
            rb.AddForce(Vector3.right * rightOvertakingXPos * mobility * force * Time.deltaTime, ForceMode.Impulse);
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

    void MoveToLane(int laneIndex)
    {
        float laneMiddle = FindObjectOfType<RoadRules>().laneMiddleX[laneIndex];
        float xDirection = laneMiddle - xPos;

        rb.AddForce(Vector3.right * xDirection * mobility * force * Time.deltaTime, ForceMode.Impulse);
    }

    void StayOnRoad()
    {
        if (xPos < -xBound)
            transform.position = new Vector3(-xBound, yPos, zPos);
        if (xPos > xBound + 4)
            transform.position = new Vector3(xBound + 4, yPos, zPos);
    }
}
