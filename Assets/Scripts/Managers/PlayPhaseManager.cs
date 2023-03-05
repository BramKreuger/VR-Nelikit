using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPhaseManager : MonoBehaviour
{
    private bool playing = false; // this bool indicates if the game has started
    private bool catching = true; // this bool indicated which role is being played. If false -> Thrower  

    public Telikit telikit;

    public GameObject thrower;
    public GameObject catcher;

    [Tooltip("Where does the thrower aim? Order matters: 0: left, 1: right, 2: up, 3: down")]
    public List<Transform> throwTargets;
    private List<Vector3> throwTargetsOffset = new List<Vector3>();

    [Tooltip("Which position to throw from")]
    public Transform throwPosition;

    [Header("Throwing Variables")]

    [Tooltip("Wait at least .. seconds before throwing. When animation has ended too.")]
    public int timeBeforeThrow = 2;
    [Tooltip("Respawn .. seconds before allowed to throw")]
    public int timeBeforeRecall = 2;    
    public int maxThrows = 10;
    public float throwForce = 5f;
    [Range(0,1)]
    public float randomForceRange = 1;
    [Range(0,1)]
    public float randomAreaRange = .5f;

    [Tooltip("At which percentage of the throw animation should the Telikit leave the hand")]
    [Range(0, 1)]
    public float throwAnimationPercentage = .3f;


    //private bool waiting = true; // are we waiting between two throws?
    private float intervalTimer = 0; // counts the time between intervals
    private int numberOfThrows = 0;
    [System.NonSerialized]
    public int validThrows = 0; // If the Telikit did reach the collider the throw is valid

    private Locomotion locomotion;

    [Tooltip("If the Telikit is within this collider it's allowed to retrieve it.")]
    public Collider retrievalCollider;

    private bool playerTurn = false;

    [Header("Everything to do with Slowing Time")]
    public float slowFactor = 2f;
    private float newTimeScale;

    private void Start()
    {
        newTimeScale = newTimeScale = Time.timeScale / slowFactor;
        locomotion = thrower.GetComponent<Locomotion>();

        for (int i = 0; i < throwTargets.Count - 1; i++)
        {
            throwTargetsOffset.Add(catcher.transform.position - throwTargets[i].position);
        }
    }

    /// <summary>
    /// This is called from the GameStateManager. Fires one time.
    /// </summary>
    /// <param name="role">Role indicates player's role.</param>
    public void StartGame()
    {
        playing = true;
        locomotion.started = true;
        intervalTimer = 0;
    }

    void CatchingGame()
    {
        ThrowTargetsFollowPlayer();

        CheckRespawnTelikit();

        // Only after throwing do we start "waiting"
        // We're not holding the telekit and we've reached our target. 
        // The target is the telikit
        // Therefore start the timer
        if (locomotion.holdingTelikit == false && locomotion.reachedTarget == true && locomotion.targetTelikit == true && CanRetrieve() == true)
        {
            intervalTimer += Time.deltaTime; // Wait ..x.. seconds before running to the telikit
            if (intervalTimer > timeBeforeRecall) // We're finished waiting, retrieve the telikit.
            {
                ResetTimeScale();
                intervalTimer = 0;
                telikit.trail.enabled = false;

                // Alow player to grab the telikit and throw it back.
                telikit.interactible.enabled = true;
                if (telikit.grounded && telikit.thrownByThrower == false) 
                {
                    locomotion.ResetTarget(telikit.transform, true); // This sets reachedTarget = false, so can only fire once <-- guy runs after telikit
                }
            }
        }
        // We're allowed to throw, based on nr of throws
        // We're holding the Telikit
        // We've reached our target
        // Therefore, start waiting a bit before throwing
        if (CanThrow() == true && locomotion.holdingTelikit == true && locomotion.reachedTarget == true)
        {
            intervalTimer += Time.deltaTime; // Wait ..x.. seconds before throwing
            locomotion.rotateTowardsPlayer = true;
            //locomotion.RotateTowards(catcher.transform.position - thrower.transform.position); // In the meantime rotate the thrower towards the player
            if (intervalTimer > timeBeforeThrow) // We're finished waiting, throw
            {
                locomotion.rotateTowardsPlayer = false;
                intervalTimer = 0;
                locomotion.targetTelikit = true; // After throwing our new target will be the telikit
                locomotion.Throw();
            }

        }
    }

    /// <summary>
    /// Check if the telikit has been thrown. Respawn it if necceserry
    /// </summary>
    void CheckRespawnTelikit()
    {
        bool respawn = false;

        if(locomotion.holdingTelikit == false)
        {
            if (telikit.rigid.velocity.magnitude < 0.1f && retrievalCollider.bounds.Contains(telikit.transform.position) == false) // The telikit has stopped moving and is out of bounds
            {
                respawn = true;
            }
            if (telikit.transform.position.y < 0) // or it's out of bounds or it's below ground
            {
                respawn = true;
            }

            if (respawn)
            {
                Debug.Log("Respawn Telikit: Velocity: " + telikit.rigid.velocity.magnitude + " bounds: " + retrievalCollider.bounds.Contains(telikit.transform.position) + " Pos Y: " + telikit.transform.position.y);
                // Depending on the state of the game (did the player throw or the NPC) 
                // Respawn the telikit at a specific place
                if (playerTurn)
                {
                    RespawnTelikit(catcher.transform);
                }
                else
                {
                    RespawnTelikit(thrower.transform);
                }
            }
        }
    }

    void RespawnTelikit(Transform _target)
    {
        telikit.trail.enabled = false;
        telikit.transform.position = _target.position + new Vector3(0, 1, 1);
        telikit.rigid.velocity = Vector3.zero;
        telikit.trail.enabled = true;
    }
    
    bool CanRetrieve()
    {
        bool withinBounds = retrievalCollider.bounds.Contains(telikit.transform.position);
        if (withinBounds == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SlowDownTime(float slowFactor)
    {
        //assign the 'newTimeScale' to the current 'timeScale'  
        Time.timeScale = newTimeScale;
        //proportionally reduce the 'fixedDeltaTime', so that the Rigidbody simulation can react correctly  
        Time.fixedDeltaTime = Time.fixedDeltaTime / slowFactor;
        //The maximum amount of time of a single frame  
        Time.maximumDeltaTime = Time.maximumDeltaTime / slowFactor;

    }

    void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.01f;
        Time.maximumDeltaTime = 0.33f;
    }

    bool CanThrow()
    {
        return numberOfThrows < maxThrows;
    }

    /// <summary>
    /// Make the AI throw a Telikit.
    /// </summary>
    public void Throw()
    {
        playerTurn = true;
        telikit.thrownByThrower = true;
        telikit.trail.enabled = true;
        intervalTimer = 0;
        numberOfThrows++;

        // Choose random throw position
        int randomInt = Random.Range(0, throwTargets.Count);
        Vector3 throwTarget = throwTargets[randomInt].position;

        if(randomInt < 2) // If it's a side throw:
        {
            telikit.transform.rotation = Quaternion.Euler(0, 0, 0); // set the rotation correct
        }
        else
        {
            telikit.transform.rotation = Quaternion.Euler(90, 0, 0); // set the rotation correct
        }

        float randomForce = Random.Range(0, randomForceRange);
        float force = throwForce + randomForce;

        float randomX = Random.Range(-randomAreaRange, randomAreaRange);
        float randomY = Random.Range(-randomAreaRange, randomAreaRange);
        float randomZ = Random.Range(-randomAreaRange, randomAreaRange);

        throwTarget = throwTarget + new Vector3(randomX, randomY, randomZ);
        
        telikit.rigid.AddForce((throwTarget - telikit.transform.position) * force, ForceMode.Impulse);
        Debug.Log("Trow: " + numberOfThrows + " Position: " + throwTargets[randomInt].name + " Random Force: " + randomForce + " Random Area: " + new Vector3(randomX, randomY, randomZ));
        SlowDownTime(slowFactor);
    }

    /// <summary>
    /// Make sure the "Throw targets" are following the player but don't rotate with the player.
    /// </summary>
    void ThrowTargetsFollowPlayer()
    {
        for (int i = 0; i < throwTargetsOffset.Count; i++)
        {
            throwTargets[i].position = catcher.transform.position - throwTargetsOffset[i];
        }
    }

    private void Update()
    {
        if (playing && catching)
        {
            CatchingGame();
        }
        // else if(playing && !catching) ... etc ...
    }
}
