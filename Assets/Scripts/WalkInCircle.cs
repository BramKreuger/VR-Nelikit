using UnityEngine;
using System.Collections;
public class WalkInCircle : MonoBehaviour
{
    public Transform[] waypoints; // array of waypoints to follow
    public float speed = 1f; // speed of the character's movement
    public float rotationSpeed = 5f; // speed of character's rotation

    private int currentWaypoint = 0; // the current waypoint the character is moving towards
    Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // check if the character has reached the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
        {
            // if reached, move to the next waypoint
            currentWaypoint++;
            // if reached the end of the waypoints, start over
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }

        /*float groundHeight = 0;
        RaycastHit hit;
        if(Physics.Raycast(transform.position + new Vector3(0,5,0), Vector3.down, out hit))
        {
            if(hit.transform.name == "Terrain")
            {
                groundHeight = hit.point.y;
            }
        }*/

        // move the character towards the current waypoint
        //Vector3 target = new Vector3(waypoints[currentWaypoint].position.x, groundHeight, waypoints[currentWaypoint].position.z);
        //transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
        //transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

        Vector3 target = waypoints[currentWaypoint].position;
        
        // update the rotation of the character to face the direction of the path
        Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
        
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up); // keep the up axis as the vertical axis
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}