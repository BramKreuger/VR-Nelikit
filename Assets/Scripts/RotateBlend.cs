using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBlend : MonoBehaviour
{
    public Animator animator;
    private float angle;
    public Transform target;


    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        angle = Vector3.Angle(direction, transform.forward);
        angle = angle / 180f * AngleDir(transform.forward, direction, Vector3.up);
        Debug.DrawRay(transform.position, direction);
        animator.SetFloat("angle", angle);
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }
}
