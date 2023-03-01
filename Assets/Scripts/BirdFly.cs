using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFly : MonoBehaviour
{
    public float flySpeed;
    public float rotatationSpeed;
    public float angle = 45f;
    public Transform center;

    void Start()
    {
        if (angle < 0)
        {
            transform.rotation = Quaternion.Euler(-180, 0, -angle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    void Update()
    {
        transform.RotateAround(center.position, Vector3.up, angle * flySpeed * Time.deltaTime);
    }
}