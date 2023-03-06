using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    public Collider retrievalCollider;
    private Rigidbody rigid;
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void CheckRespawnTelikit()
    {
        bool respawn = false;

        if (rigid.velocity.magnitude < 0.1f && retrievalCollider.bounds.Contains(transform.position) == false) // The telikit has stopped moving and is out of bounds
        {
            respawn = true;
        }
        if (transform.position.y < 0) // or it's out of bounds or it's below ground
        {
            respawn = true;
        }

        if (respawn)
        {
            transform.position = new Vector3(player.position.x, player.position.y + 1, player.position.z + 1);
        }
    }
}
