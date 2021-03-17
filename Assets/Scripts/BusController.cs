using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    public float maxSpeed_kmh;
    public float force;
    public float mobility;

    private float maxSpeed;
    private float detDist = 20;
    private float xPos;
    private float yPos;
    private float zPos;
    private float playerSpeed;
    private Rigidbody rb;
    private RaycastHit hit; // stores the information about collided objects

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxSpeed = maxSpeed_kmh * 1000 / 2600 / 25 * force; // km/h -> m/frame * force
    }

    // Update is called once per frame
    void Update()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;
        playerSpeed = Input.GetAxis("Vertical") * 1.9f * force;

        AvoidObstacles();
        HoldTheLane();
    }

    void AvoidObstacles()
    {
        Vector3 rayStart1 = new Vector3(xPos, 0.1f, zPos);
        Ray crachRay1 = new Ray(rayStart1, Vector3.forward); // release a ray1
        Debug.DrawRay(rayStart1, Vector3.forward * detDist);

        Vector3 rayStart2 = new Vector3(xPos - 1, 0.1f, zPos);
        Ray crachRay2 = new Ray(rayStart1, Vector3.forward); // release a ray1
        Debug.DrawRay(rayStart2, Vector3.forward * detDist);

        Vector3 rayStart3 = new Vector3(xPos + 1, 0.1f, zPos);
        Ray crachRay3 = new Ray(rayStart1, Vector3.forward); // release a ray1
        Debug.DrawRay(rayStart3, Vector3.forward * detDist);

        if (Physics.Raycast(crachRay1, out hit, detDist) &&
            Physics.Raycast(crachRay2, out hit, detDist) &&
            Physics.Raycast(crachRay3, out hit, detDist))
        {
            if (hit.collider.CompareTag("Traffic Car"))
            {
                MoveForward(0);
            }
        }
        else
        {
            MoveForward(maxSpeed);
        }
    }

    void MoveForward(float speed)
    {
        rb.AddForce(Vector3.forward * (speed - playerSpeed) * Time.deltaTime, ForceMode.Impulse);
    }

    void HoldTheLane()
    {
        rb.AddForce(Vector3.right * (6 - xPos) * force * mobility * Time.deltaTime, ForceMode.Impulse);
    }
}
