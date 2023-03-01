using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyController : MonoBehaviour
{
    public List<ButterFly> butterflies;
    public List<Transform> flowers;

    public float restingTime;

    public float flyingSpeed;
    public float rotationSpeed;
    public float flowerOffset = .3f;

    [Tooltip("0: No chance of going to a flower, 1: 100%")]
    [Range(0, 1)]
    public float flowerChance = .25f;

    private Collider boundingBox; // For the bounds of the butterflies


    // Start is called before the first frame update
    void Start()
    {
        boundingBox = GetComponent<Collider>();

        foreach (ButterFly butterfly in butterflies)
        {
            PickTarget(butterfly);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ButterFly butterfly in butterflies)
        {
            bool reachedTarget = ReachedTarget(butterfly.transform, butterfly.target);
            if(reachedTarget && butterfly.flower) // If the butterfly reached a flower.
            {
                if(butterfly.restingTimeCurrent == 0) // This fires once!
                {
                    butterfly.animator.speed = 0;
                }
                butterfly.restingTimeCurrent += Time.deltaTime; // Wait
                if(butterfly.restingTimeCurrent > restingTime) // WaitingTime is over
                {
                    butterfly.animator.speed = 1;
                    butterfly.restingTimeCurrent = 0;
                    PickTarget(butterfly);
                }
            }
            else if(reachedTarget) // Reached target
            {
                PickTarget(butterfly);
            }
            else // Fly away
            {
                FlyTowards(butterfly.transform, butterfly.target);
            }
        }
    }

    void PickTarget(ButterFly butterfly)
    {
        Vector3 target;
        bool flower = false;

        float randomChance = UnityEngine.Random.Range(0f, 1f);
        if(randomChance < flowerChance) // Pick a flower
        {
            flower = true;
            int randomFlowerInt = UnityEngine.Random.Range(0, flowers.Count);
            target = flowers[randomFlowerInt].position;
            target = new Vector3(target.x, target.y + flowerOffset, target.z);
        }
        else // Pick a random spot
        {
            target = RandomPointInBounds(boundingBox.bounds);
        }

        butterfly.target = target;
        butterfly.flower = flower;
    }


    /// <summary>
    /// Fly towards target
    /// </summary>
    /// <param name="butterfly">The object flying towards the target</param>
    /// <param name="target">Eiter a random point in space or a flower</param>
    /// <returns>Reached target?</returns>
    void FlyTowards(Transform butterfly, Vector3 target)
    {
        // Determine which direction to rotate towards
        Vector3 direction = target - butterfly.position;

        // The step size is equal to speed times frame time.
        //float singleStep = rotationSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        //Vector3 newDirection = Vector3.RotateTowards(butterfly.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        //Debug.DrawRay(butterfly.position, direction * Vector3.Distance(butterfly.position,target), Color.red);
        Debug.DrawLine(butterfly.position, target);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        //butterfly.rotation = Quaternion.LookRotation(newDirection);

        /*
        Quaternion rot = Quaternion.LookRotation(direction);
        butterfly.transform.rotation = Quaternion.Slerp(butterfly.transform.rotation, rot, Time.deltaTime * rotationSpeed);
        // Move towards target
        butterfly.position = Vector3.MoveTowards(butterfly.position, target, Time.deltaTime * flyingSpeed);
        */


        //butterfly.transform.LookAt(target);

        Quaternion rot = Quaternion.LookRotation(direction);
        butterfly.rotation = Quaternion.Slerp(butterfly.rotation, rot, Time.deltaTime * rotationSpeed);
        butterfly.Translate(Vector3.forward * flyingSpeed * Time.deltaTime, Space.Self);
    }

    bool ReachedTarget(Transform butterfly, Vector3 target)
    {
        if (Vector3.Distance(butterfly.position, target) < 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
