using UnityEngine;

// Handles player inputs: treats and commands
public class CommandManager : MonoBehaviour
{
    // Reference to the dog controller
    [SerializeField] private DogController dog;

    // Reference to fear system
    [SerializeField] private FearSystem fearSystem;

    // Reference to trust system
    [SerializeField] private TrustSystem trustSystem;

    // Reference to level manager
    [SerializeField] private Level1Manager levelManager;

    private void Start()
    {
        // Auto-find systems if not assigned
        if (fearSystem == null) fearSystem = FindObjectOfType<FearSystem>();
        if (trustSystem == null) trustSystem = FindObjectOfType<TrustSystem>();
        if (levelManager == null) levelManager = FindObjectOfType<Level1Manager>();
    }

    private void Update()
    {
        // Press T to give a treat (only works within 4 meters)
        if (Input.GetKeyDown(KeyCode.T))
        {
            levelManager.TryGiveTreat();
        }

        // Key 1 -> Sit command (only if unlocked)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            levelManager.TrySitCommand();
            fearSystem.AddFear(-1f);
        }

        // Key 2 -> Stay command (still available for now)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            dog.ExecuteCommand(DogCommand.Stay);
            fearSystem.AddFear(-1f);
        }

        // Key 3 -> Follow command (still available for now)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            dog.ExecuteCommand(DogCommand.Follow);
            fearSystem.AddFear(-1f);
        }
    }
}
