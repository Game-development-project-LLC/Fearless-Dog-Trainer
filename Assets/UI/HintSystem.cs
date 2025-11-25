using UnityEngine;
using UnityEngine.UI;

// Shows simple floating hints based on Level 1 rules
public class HintSystem : MonoBehaviour
{
    // Reference to UI text for hints
    [SerializeField] private Text hintText;

    // References to systems
    [SerializeField] private FearSystem fearSystem;
    [SerializeField] private Level1Manager level1Manager;

    // Player and dog for distance checks
    [SerializeField] private Transform player;
    [SerializeField] private Transform dog;

    private void Start()
    {
        // Auto-find if not assigned
        if (fearSystem == null) fearSystem = FindObjectOfType<FearSystem>();
        if (level1Manager == null) level1Manager = FindObjectOfType<Level1Manager>();
        if (player == null) player = GameObject.FindWithTag("Player")?.transform;
        if (dog == null) dog = GameObject.Find("Dog")?.transform;
    }

    private void Update()
    {
        if (hintText == null || fearSystem == null || level1Manager == null || player == null || dog == null)
            return;

        // Calculate distance between player and dog
        float dist = Vector3.Distance(player.position, dog.position);

        // Priority 1: high fear hint
        if (fearSystem.CurrentFear > 60f)
        {
            hintText.text = "Take a step back and breathe";
            return;
        }

        // Priority 2: treat hint when close and sit not unlocked
        if (dist <= level1Manager.TreatRange && !level1Manager.SitUnlocked)
        {
            hintText.text = "Press T to give treat";
            return;
        }

        // Priority 3: sit hint when unlocked
        if (level1Manager.SitUnlocked)
        {
            hintText.text = "Approach the dog and press 1 to sit.";
            return;
        }

        // Otherwise no hint
        hintText.text = "";
    }
}
