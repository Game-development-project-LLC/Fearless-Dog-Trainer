using UnityEngine;

// enum = list of possible commands for the dog
public enum DogCommand { Sit, Stay, Follow }

// This script controls the dog's behavior
public class DogController : MonoBehaviour
{
    // Movement speed when the dog follows the player
    [SerializeField] private float followSpeed = 3f;

    // Reference to the player Transform so the dog knows where to go
    private Transform player;

    // State flags that describe the dog's current mode
    private bool isSitting;
    private bool isStaying;
    private bool isFollowing;

    private void Awake()
    {
        // Find the GameObject tagged "Player" and store its Transform
        player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
    {
        // If the dog is in Follow mode and player exists, move towards the player every frame
        if (isFollowing && player != null)
        {
            // Create a target position only on X axis (keep same Y)
            Vector3 target = new Vector3(player.position.x, transform.position.y, 0);

            // Move the dog towards the target at fixed speed
            transform.position = Vector3.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);
        }
    }

    // Receives a command and updates the dog's state
    public void ExecuteCommand(DogCommand cmd)
    {
        // Reset all states before applying a new one
        isSitting = false;
        isStaying = false;
        isFollowing = false;

        // Choose new state based on the received command
        switch (cmd)
        {
            // Sit command: dog sits
            case DogCommand.Sit:
                isSitting = true;
                break;

            // Stay command: dog stays in place
            case DogCommand.Stay:
                isStaying = true;
                break;

            // Follow command: dog follows the player
            case DogCommand.Follow:
                isFollowing = true;
                break;
        }
    }
}
