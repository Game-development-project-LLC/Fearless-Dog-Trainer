using UnityEngine;
using UnityEngine.UI;

// Level 1 rules based on the design document
public class Level1Manager : MonoBehaviour
{
    [Header("References")]
    // Reference to player transform
    [SerializeField] private Transform player;

    // Reference to dog transform
    [SerializeField] private Transform dog;

    // Direct reference to the dog's controller (no FindObjectOfType)
    [SerializeField] private DogController dogController;

    // Reference to fear system
    [SerializeField] private FearSystem fearSystem;

    // Reference to trust system
    [SerializeField] private TrustSystem trustSystem;

    [Header("UI")]
    // Optional text for messages/hints
    [SerializeField] private Text resultText;

    [Header("Level Rules")]
    // Treat range (meters)
    [SerializeField] private float treatRange = 4f;

    // Sit success range (meters)
    [SerializeField] private float sitRange = 3f;

    // Treat trust gain
    [SerializeField] private float trustPerTreat = 15f;

    // Trust penalty on early Sit
    [SerializeField] private float trustPenaltyEarlySit = 5f;

    // Required trust to allow Sit
    [SerializeField] private float minTrustToSit = 30f;

    // Required trust to win
    [SerializeField] private float trustToWin = 50f;

    // Level time limit in seconds (3 minutes)
    [SerializeField] private float timeLimit = 180f;

    // Fear threshold for failure timer
    [SerializeField] private float fearFailThreshold = 70f;

    // How long fear can stay above threshold before failing
    [SerializeField] private float fearFailDuration = 3f;

    // Treats in a row needed to unlock Sit
    [SerializeField] private int treatsToUnlockSit = 2;

    [Header("Optional Behaviors")]
    // How far the dog steps back on failed sit
    [SerializeField] private float dogBackOffDistance = 1f;

    // How long the sit visual stays before resetting
    [SerializeField] private float sitVisualDuration = 1.5f;

    // Counters
    private int consecutiveTreats;
    private bool sitUnlocked;

    // Timers
    private float levelTimer;
    private float fearAboveTimer;

    // Flag to stop level once ended
    private bool levelEnded;

    // Store dog's original scale for visual reset
    private Vector3 dogOriginalScale;

    private void Start()
    {
        // Make sure time is running at level start
        Time.timeScale = 1f;

        // Auto-find references if missing
        if (player == null) player = GameObject.FindWithTag("Player")?.transform;
        if (dog == null) dog = GameObject.Find("Dog")?.transform;
        if (fearSystem == null) fearSystem = FindObjectOfType<FearSystem>();
        if (trustSystem == null) trustSystem = FindObjectOfType<TrustSystem>();

        // If dogController not assigned, try to get it from the dog object
        if (dogController == null && dog != null)
            dogController = dog.GetComponent<DogController>();

        // Save original dog scale for sit reset
        if (dog != null)
            dogOriginalScale = dog.localScale;

        // Hide result text at start
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Stop updating if level ended
        if (levelEnded) return;

        // Update level timer
        levelTimer += Time.deltaTime;

        // Check fear-over-70 duration
        CheckFearFailure();

        // Check time limit win condition
        CheckWinCondition();

        // If time is over and not won -> lose
        if (levelTimer >= timeLimit && trustSystem.CurrentTrust < trustToWin)
        {
            EndLevel(false, "TIME OVER - YOU LOSE");
        }
    }

    // Called when player presses T
    public void TryGiveTreat()
    {
        // Do nothing if level ended
        if (levelEnded) return;

        // Check distance between player and dog
        float dist = Vector3.Distance(player.position, dog.position);

        // Treat only allowed within 4 meters
        if (dist <= treatRange)
        {
            // Increase trust
            trustSystem.AddTrust(trustPerTreat);

            // Count consecutive treats
            consecutiveTreats++;

            // Unlock Sit after 2 treats in a row
            if (consecutiveTreats >= treatsToUnlockSit)
                sitUnlocked = true;
        }
        else
        {
            // If too far, reset treat streak
            consecutiveTreats = 0;
        }
    }

    // Called when player presses 1
    public void TrySitCommand()
    {
        // Do nothing if level ended
        if (levelEnded) return;

        // Sit not unlocked yet
        if (!sitUnlocked) return;

        // Check distance for sit success
        float dist = Vector3.Distance(player.position, dog.position);

        // Sit succeeds only if close enough and trust >= 30
        if (dist < sitRange && trustSystem.CurrentTrust >= minTrustToSit)
        {
            // Execute sit command on the dog controller
            if (dogController != null)
                dogController.ExecuteCommand(DogCommand.Sit);

            // Simple visual feedback: "sit" by squashing the dog sprite a bit
            if (dog != null)
            {
                dog.localScale = new Vector3(dogOriginalScale.x, dogOriginalScale.y * 0.3f, dogOriginalScale.z);
                StartCoroutine(ResetSitVisualAfterDelay());
            }
        }
        else
        {
            // Early/incorrect sit -> trust penalty and reset treat streak
            trustSystem.AddTrust(-trustPenaltyEarlySit);
            consecutiveTreats = 0;

            // Dog backs off a bit from the player (simple step)
            BackOffDog();
        }
    }

    // Resets the dog scale after a short delay
    private System.Collections.IEnumerator ResetSitVisualAfterDelay()
    {
        // Wait for visual duration
        yield return new WaitForSeconds(sitVisualDuration);

        // Reset to original scale
        if (dog != null)
            dog.localScale = dogOriginalScale;
    }

    // Moves the dog slightly away from the player
    private void BackOffDog()
    {
        if (dog == null || player == null) return;

        // Determine direction away from player on X axis
        float dir = Mathf.Sign(dog.position.x - player.position.x);

        // Step away
        dog.position += new Vector3(dir * dogBackOffDistance, 0f, 0f);
    }

    private void CheckFearFailure()
    {
        // If fear above threshold, count time above
        if (fearSystem.CurrentFear > fearFailThreshold)
        {
            fearAboveTimer += Time.deltaTime;

            // Fail if stayed above for too long
            if (fearAboveTimer >= fearFailDuration)
            {
                EndLevel(false, "FEAR TOO HIGH - YOU LOSE");
            }
        }
        else
        {
            // Reset timer if fear drops back
            fearAboveTimer = 0f;
        }
    }

    private void CheckWinCondition()
    {
        // Win if trust >= 50 within time limit
        if (trustSystem.CurrentTrust >= trustToWin && levelTimer <= timeLimit)
        {
            EndLevel(true, "YOU WIN!");
        }
    }

    private void EndLevel(bool won, string message)
    {
        // Mark as ended
        levelEnded = true;

        // Show result text
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = message;
        }

        // Pause the game at end
        Time.timeScale = 0f;
    }

    // ===== Public getters for HintSystem =====
    public bool SitUnlocked => sitUnlocked;
    public int ConsecutiveTreats => consecutiveTreats;
    public float TreatRange => treatRange;
    public float SitRange => sitRange;
}
