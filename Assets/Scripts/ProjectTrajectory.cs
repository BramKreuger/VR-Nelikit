using UnityEngine;

public class ProjectTrajectory : MonoBehaviour
{
    // The rigidbody to simulate the trajectory for
    private Rigidbody objectRigidbody;


    // The speed of the simulation
    public float simulationSpeed = 2.0f;

    // The number of points along the trajectory
    public int resolution = 30;

    // The LineRenderer component used to display the trajectory
    private LineRenderer lineRenderer;

    // An array to store the positions of the trajectory
    private Vector3[] points;

    void Start()
    {
        // Get a reference to the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        // do the same for the rigidbody
        objectRigidbody = GetComponent<Rigidbody>();

        // Initialize the points array
        points = new Vector3[resolution];
    }

    void Update()
    {
        // Store the starting position and velocity of the rigidbody
        Vector3 startPosition = objectRigidbody.position;
        Vector3 startVelocity = objectRigidbody.velocity;

        // Keep track of the elapsed time
        float elapsedTime = 0;

        // Calculate the trajectory of the rigidbody
        for (int i = 0; i < resolution; i++)
        {
            // Increase the elapsed time
            elapsedTime += Time.fixedDeltaTime * simulationSpeed;

            // Calculate the position of the rigidbody at the current elapsed time
            points[i] = startPosition + startVelocity * elapsedTime + 0.5f * Physics.gravity * elapsedTime * elapsedTime;
        }

        // Update the LineRenderer with the positions of the trajectory
        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(points);
    }
}
