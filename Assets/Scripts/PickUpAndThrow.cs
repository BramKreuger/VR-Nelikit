using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAndThrow : MonoBehaviour
{
    public Telikit telikit;
    public Vector3 posAttatchOffset;
    private Collider coll;
    bool telikitAttatched;
    float velocity = 1f;
    float g; // gravity

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();
        g = Mathf.Abs(Physics.gravity.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (telikitAttatched)
            Carry();

        //Pick up telikit
        if(Input.GetMouseButtonDown(0) && telikitAttatched == false)
        {
            PickUp();
        }
        if(Input.GetMouseButtonDown(0))// && telikitAttatched == true)
        {
            Aim();
        }
    }

    void PickUp()
    {
        if (coll.bounds.Contains(telikit.transform.position))
        {
            telikitAttatched = true;
            telikit.constraint.enabled = false;
            telikit.trail.enabled = false;
        }
        
    }
    void Carry()
    {
        telikit.transform.position = transform.position + posAttatchOffset;
        telikit.transform.rotation = transform.rotation;
    }
    void Aim()
    {
    }

}
