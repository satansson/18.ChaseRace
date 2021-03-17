using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float mobility;
    public float force;    

    private float xBound = 8.5f;
    private float zBound = 14f;

    private Rigidbody playerRb;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        StopPlayer();
        ActPlayer(); 
    }

    void MovePlayer()
    {
        float horIn = Input.GetAxis("Horizontal");
        float vertIn = Input.GetAxis("Vertical");

        speed = vertIn * 1.9f;
        transform.Translate(Vector3.right * horIn * mobility * Time.deltaTime);
    }

    void StopPlayer()
    {
        if (transform.position.x < -xBound)
        {
            transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
        }

        if (transform.position.x > xBound)
        {
            transform.position = new Vector3(xBound, transform.position.y, transform.position.z);
        }

        if (transform.position.z < -zBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -zBound);
        }

        if (transform.position.z > zBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBound);
        }
        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void ActPlayer()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Wheelie");
            //TODO logic that can turn player
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("RearBrake");
            //TODO logic that can turn player
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cop"))
        {
            Debug.Log("+ Reaction, + Speed");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fuel"))
        {
            Destroy(other.gameObject);
            Debug.Log("Fuel has picked up");
        }
    }
}
