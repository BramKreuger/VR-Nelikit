using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using System;

public class Locomotion : MonoBehaviour
{
    [Tooltip("Set this as the Telikit")]
    public Transform target;
    Telikit telikit;

    // All the movement stuff
    public float distanceToIdle = .2f;
    private Animator animator;
    float velocityX;
    float velocityZ;
    float angle;
    public float rotationSpeed;

    // Animations
    public ChainIKConstraint armIK;
    public float armLerpDuration = .5f;
    bool pickingUp = false;

    public MultiAimConstraint headIK;
    public float headLerpDuration = 1f;

    // Logics
    public PlayPhaseManager playPhaseManager;
    public bool holdingTelikit = false;
    public bool targetTelikit = true;
    public bool reachedTarget = true;
    public Transform startingPoint; // Go back here once you've retrieved the telikit.
    public Transform player;
    public bool rotateTowardsPlayer = false;
    [System.NonSerialized]
    public bool started = false;
    

    //Vector2 smoothDeltaPosition = Vector2.zero;
    //Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        telikit = target.GetComponent<Telikit>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (started)
        {
            UpdateCharacter();
        }
    }

    // Update is called once per frame
    void UpdateCharacter()
    {
        StayGrounded();
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        Vector3 direction;
        if (info.IsName("StationaryRotation") && rotateTowardsPlayer) //We are rotating stationary and towards the player
        {
            // Rotate towards the player
            direction = player.position - transform.position;

        }
        else
        {
            direction = target.position - transform.position;
        }

        float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
        SetAnimParams(direction, distance);

        if (reachedTarget == false)
        {         
            if (distance < distanceToIdle) // Close enough to do stop
            {
                animator.SetBool("ReachedTarget", true);
                reachedTarget = true;
                Debug.Log("Reached. TargetTelikit: " + targetTelikit);

                if (pickingUp == false && holdingTelikit == false && targetTelikit == true) // This is a one-shot. Only if the the target is the telikit
                {                        
                    pickingUp = true;
                    StartPickingUp();
                }
            }
            if (velocityZ < -0.5f || velocityZ > 0.5f && info.IsName("Motion"))
            {
                RotateTowards(direction);
            }
        }

        // This is to check when the anims are at a certain time. Should be changed
        if(info.IsName("Throw") && info.normalizedTime > .3f && telikit.constraint.enabled)
        {
            ReleaseTelikit();
            playPhaseManager.Throw();
        }
        if (info.IsName("Pick_up") && info.normalizedTime > .4f && pickingUp == true)
        {
            PickUp();
        }
    }

    void StayGrounded()
    {
        float groundHeight = 0;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 5, 0), Vector3.down, out hit))
        {
            if (hit.transform.name == "Terrain")
            {
                groundHeight = hit.point.y;
            }
        }
        transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
    }

    public void SetAnimParams(Vector3 direction, float distance)
    {
        angle = Vector3.Angle(direction, transform.forward);
        angle = angle / 180f * AngleDir(transform.forward, direction, Vector3.up);
        Debug.DrawRay(transform.position, direction);
        animator.SetFloat("angle", angle);
        velocityX = Vector3.Dot(transform.right, direction);
        velocityZ = Vector3.Dot(transform.forward, direction);
        animator.SetFloat("velocity_x", velocityX);
        animator.SetFloat("velocity_z", velocityZ);
        animator.SetFloat("distance", distance);
    }

    public void RotateTowards(Vector3 direction)
    {
        Quaternion LookAtRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, LookAtRotationOnly_Y, Time.deltaTime * rotationSpeed);
    }

    /// <summary>
    /// This can be accessed from outside to reset the target. Reenables movement.
    /// </summary>
    public void ResetTarget(Transform _target, bool isTelikit)
    {
        Debug.Log("Reset");
        targetTelikit = isTelikit;
        reachedTarget = false;
        animator.SetBool("ReachedTarget", false);
        target = _target;
    }

    /// <summary>
    /// return -1 for angle left, 1 for angle right
    /// </summary>
    /// <param name="fwd">Forward of the transform</param>
    /// <param name="targetDir">direction to target</param>
    /// <param name="up">what is up?</param>
    /// <returns></returns>
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

    void StartPickingUp()
    {
        StartCoroutine(ArmLerp(0, 1, armLerpDuration));
        StartCoroutine(HeadLerp(1, 0, headLerpDuration)); // Look at telikit
        animator.SetTrigger("pickUp");      

    }

    /// <summary>
    /// The moment the Telikit goes into the player's hand. Gets triggered by the animation
    /// </summary>
    public void PickUp()
    {
        pickingUp = false;
        telikit.constraint.constraintActive = true;
        telikit.constraint.enabled = true;
        holdingTelikit = true;
        StartCoroutine(ArmLerp(1, 0, armLerpDuration / 2));
        ResetTarget(startingPoint, false);
        Debug.Log("Pick up Telikit");
    }

    public void Throw()
    {
        holdingTelikit = false;
        animator.SetTrigger("throw");
        StartCoroutine(HeadLerp(0, 1, headLerpDuration)); // Look at player
    }

    public void ReleaseTelikit()
    {
        telikit.constraint.constraintActive = false;
        telikit.constraint.enabled = false;
    }

    IEnumerator ArmLerp(float startValue, float endValue, float lerpDuration)
    {
        float armLerp = startValue;
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            armLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            armIK.weight = armLerp;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        armLerp = endValue;
        armIK.weight = armLerp;
    }

    /// <summary>
    /// Learp the weight of the head source objects. [0] is the telikit, [1] is the player
    /// </summary>
    /// <param name="startValue">start</param>
    /// <param name="endValue">end</param>
    /// <param name="lerpDuration">how long</param>
    /// <returns></returns>
    IEnumerator HeadLerp(float startValue, float endValue, float lerpDuration)
    {
        Debug.Log("Lerp");
        var sources = headIK.data.sourceObjects;
        float headLerp_0_1;
        float headLerp_1_0;
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            headLerp_0_1 = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            headLerp_1_0 = Mathf.Lerp(endValue, startValue, timeElapsed / lerpDuration);            
            sources.SetWeight(0, headLerp_0_1);
            sources.SetWeight(1, headLerp_1_0);
            headIK.data.sourceObjects = sources;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        sources.SetWeight(0, endValue);
        sources.SetWeight(1, startValue);
        headIK.data.sourceObjects = sources;
    }
}
