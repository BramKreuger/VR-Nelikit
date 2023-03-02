using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// This scripts holds all the variables and objects related to the Telikit. Easily accessible from other scripts.
/// </summary>
public class Telikit : MonoBehaviour
{
    [System.NonSerialized]
    public Rigidbody rigid;
    [System.NonSerialized]
    public TrailRenderer trail;
    [System.NonSerialized]
    public ParentConstraint constraint;
    [System.NonSerialized]
    public XRGrabInteractable interactible;

    public bool thrownByThrower = false;
    public bool grounded = false;

    private void Start()
    {
        
        rigid = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        constraint = GetComponent<ParentConstraint>();
        interactible = GetComponent<XRGrabInteractable>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide with: " + collision.gameObject.name);
        if(collision.transform.CompareTag("terrain"))
        {
            grounded = true; 
            if(thrownByThrower == false && interactible.enabled == true) // so it's been thrown by the player
            {
                interactible.enabled = false;
            }
            else if(thrownByThrower == true && interactible.enabled == false)
            {
                interactible.enabled = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("terrain"))
        {
            grounded = false;
        }
    }

    public void PlayerThrow()
    {
        Debug.Log("supposed throw");
        thrownByThrower = false;
    }
}