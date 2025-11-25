using UnityEngine;
using UnityEngine.UI;

// This script manages the Fear value and Idle Calm behavior
public class FearSystem : MonoBehaviour
{
    // Initial fear level
    [SerializeField] private float fear = 40f;

    // Minimum and maximum bounds for fear
    [SerializeField] private float minFear = 0f;
    [SerializeField] private float maxFear = 100f;

    // Reference to the fear value text
    [SerializeField] private Text fearValueText;


    [Header("Idle Calm")]
    // Seconds of no movement before calming starts
    [SerializeField] private float idleTimeToCalm = 4f;

    // Fear reduction per second while calming (4% per second)
    [SerializeField] private float calmRatePerSec = 4f;

    // Fear will not go below this value during calming
    [SerializeField] private float calmMinFear = 40f;

    [Header("UI")]
    // Reference to the Fear slider in the Canvas
    [SerializeField] private Slider fearSlider;

    // Timer that counts how long the player is idle
    private float idleTimer;

    // Last recorded player position to detect movement
    private Vector3 lastPlayerPos;

    // Reference to the player Transform
    private Transform player;

    private void Start()
    {
        // Find the player by Tag and store its Transform
        player = GameObject.FindWithTag("Player")?.transform;

        // If player exists, store initial position
        if (player != null) lastPlayerPos = player.position;

        // Update UI once at start
        UpdateUI();
    }

    private void Update()
    {
        // Track whether the player is idle or moving
        TrackIdle();

        // Update slider every frame
        UpdateUI();
    }

    private void TrackIdle()
    {
        // If there is no player, do nothing
        if (player == null) return;

        // Check if the player moved since last frame
        bool moved = Vector3.Distance(player.position, lastPlayerPos) > 0.001f;

        // If moved, reset idle timer
        if (moved)
        {
            idleTimer = 0f;
            lastPlayerPos = player.position;
        }
        else
        {
            // If not moved, increase idle timer
            idleTimer += Time.deltaTime;

            // If idle time passed the threshold, start calming
            if (idleTimer >= idleTimeToCalm)
            {
                // Reduce fear at calm rate
                fear -= calmRatePerSec * Time.deltaTime;

                // Clamp fear so it won't go below calmMinFear or above maxFear
                fear = Mathf.Clamp(fear, calmMinFear, maxFear);
            }
        }
    }

    // Public method to modify fear from other scripts
    public void AddFear(float amount)
    {
        // Apply fear change and clamp to bounds
        fear = Mathf.Clamp(fear + amount, minFear, maxFear);

        // Reset idle timer after fear changes
        idleTimer = 0f;
    }

    // Public getter for current fear value
    public float CurrentFear => fear;

    private void UpdateUI()
    {
        // If slider is assigned, update its value
        if (fearSlider != null)
            fearSlider.value = fear;

        // Update fear value text if assigned
        if (fearValueText != null)
            fearValueText.text = Mathf.RoundToInt(fearSlider.value).ToString();

    }
}
