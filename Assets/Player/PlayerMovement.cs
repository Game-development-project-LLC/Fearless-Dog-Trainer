using UnityEngine;

// Handles horizontal player movement and increases fear only on fast movement towards the dog
public class PlayerMovement : MonoBehaviour
{
    // Movement speed in units per second
    [SerializeField] private float moveSpeed = 4f;

    // If player's speed is above this threshold, fear may increase
    [SerializeField] private float sharpMoveSpeedThreshold = 6f;

    // How much fear to add per second while moving too fast towards the dog
    [SerializeField] private float fearIncreasePerSec = 6f;

    // Reference to FearSystem
    [SerializeField] private FearSystem fearSystem;

    // Reference to the dog Transform
    [SerializeField] private Transform dog;

    // Stores last frame position to calculate speed and direction
    private Vector3 lastPos;

    private void Start()
    {
        // If fearSystem not assigned, find it in the scene
        if (fearSystem == null)
            fearSystem = FindObjectOfType<FearSystem>();

        // If dog not assigned, find it by name in the scene
        if (dog == null)
            dog = GameObject.Find("Dog")?.transform;

        // Save initial position
        lastPos = transform.position;
    }

    private void Update()
    {
        // Read horizontal input (A/D or Left/Right)
        float x = Input.GetAxisRaw("Horizontal");

        // Create input direction vector (horizontal only)
        Vector3 dir = new Vector3(x, 0f, 0f).normalized;

        // Move player by direction * speed * deltaTime
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Calculate movement vector since last frame
        Vector3 movement = transform.position - lastPos;

        // Calculate distance moved since last frame
        float dist = movement.magnitude;

        // Calculate current speed (units per second)
        float currentSpeed = dist / Time.deltaTime;

        // Check if player is moving fast enough to be considered "sharp"
        bool isSharpMove = currentSpeed > sharpMoveSpeedThreshold;

        // By default assume not moving towards the dog
        bool movingTowardDog = false;

        // If dog exists and there was movement, check direction toward dog
        if (dog != null && dist > 0.0001f)
        {
            // Vector from player to dog
            Vector3 toDog = (dog.position - transform.position).normalized;

            // Movement direction this frame
            Vector3 moveDir = movement.normalized;

            // Dot product > 0 means movement is in the general direction of the dog
            movingTowardDog = Vector3.Dot(moveDir, toDog) > 0f;
        }

        // Increase fear only if movement is sharp AND towards the dog
        if (isSharpMove && movingTowardDog)
        {
            // Add fear proportional to time
            fearSystem.AddFear(fearIncreasePerSec * Time.deltaTime);
        }

        // Update last position for next frame
        lastPos = transform.position;
    }
}
