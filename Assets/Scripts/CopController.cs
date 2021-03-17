using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopController : MonoBehaviour
{
    public float reactDistance;
    public float atackDistance;
    public float speed;

    private readonly float xBound = 8.5f;
    private Rigidbody rb;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -xBound)
            transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
        if (transform.position.x > xBound)
            transform.position = new Vector3(xBound, transform.position.y, transform.position.z);

        float playerSpeed = Input.GetAxis("Vertical") * 1.9f;
        float xDirection = player.transform.position.x - transform.position.x;
        float zDirection = player.transform.position.z - transform.position.z;

        transform.Translate(Vector3.back * playerSpeed);

        if (transform.position.z < reactDistance)
        {
            rb.AddForce(Vector3.right * xDirection * speed * Time.deltaTime, ForceMode.Impulse);      
        }

        if (zDirection < atackDistance && zDirection > -atackDistance)
        {
            rb.AddForce(Vector3.forward * zDirection * speed * Time.deltaTime, ForceMode.Impulse);
        }

        if (transform.position.y < 1)
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }
}
