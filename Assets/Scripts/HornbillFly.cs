using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornbillFly : MonoBehaviour
{
    bool flying = true;
    public float waitingTime = 10f;
    public List<Transform> waypoints;
    [TooltipAttribute("This lenght should match the length of the waypoints. Is this waypoint cosidered a stop?")]
    public List<bool> waypointStop;
    public Transform player;
    public float speed = 1f; // speed of the character's movement
    public float rotationSpeed = 5f; // speed of character's rotation

    private int currentWaypoint = 0; // the current waypoint the character is moving towards
    private float currentWaitingTime = 0f;

    private Animator anim;

    public float screamInterval = 60f;
    private float currentScreamWait = 0f;
    public AudioSource screamingSound;
    public AudioSource flappingSound;

    private void Start()
    {
        anim  = GetComponent<Animator>();
        //screamingSound = GetComponent<AudioSource>(); 

        if(waypoints.Count != waypointStop.Count)
        {
            Debug.LogError("Waypoints count and waypointstop count should have same length");
        }
    }
    void Update()
    {
        if (flying)
        {
            // check if the character has reached the current waypoint
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
            {
                if (waypointStop[currentWaypoint] == true)
                {
                    flying = false;
                    anim.SetBool("waiting", true);
                    flappingSound.Stop();
                }
                // if reached, move to the next waypoint
                currentWaypoint++;
                // if reached the end of the waypoints, start over
                if (currentWaypoint >= waypoints.Count)
                {
                    currentWaypoint = 0;
                }                
            }

            // move the character towards the current waypoint
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, Time.deltaTime * speed);

            // update the rotation of the character to face the direction of the path
            Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up); // keep the up axis as the vertical axis
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            currentWaitingTime += Time.deltaTime;

            // update the rotation of the character to face the direction of the player
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up); 
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (rotationSpeed + 2));

            if (currentWaitingTime > waitingTime)
            {
                anim.SetBool("waiting", false);
                currentWaitingTime = 0;
                flying = true;
            }
        }

        currentScreamWait += Time.deltaTime;
        if(currentScreamWait > screamInterval)
        {
            Scream();
            currentScreamWait = 0;
        }
    }

    public void Scream()
    {
        anim.SetTrigger("scream");
        screamingSound.Play();
    }

    public void Flap()
    {
        flappingSound.Play();
    }

}
