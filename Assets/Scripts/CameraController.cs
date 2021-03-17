using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position + new Vector3(0, 10, -5);
    }
}
