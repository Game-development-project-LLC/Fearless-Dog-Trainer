using UnityEngine;
using UnityEngine.UI;

// Manages the dog's trust value
public class TrustSystem : MonoBehaviour
{
    // Initial trust level
    [SerializeField] private float trust = 0f;

    // Reference to the trust value text
    [SerializeField] private Text trustValueText;
    
    // Minimum and maximum bounds for trust
    [SerializeField] private float minTrust = 0f;
    [SerializeField] private float maxTrust = 100f;

    [Header("UI")]
    // Reference to the Trust slider
    [SerializeField] private Slider trustSlider;




    private void Start()
    {
        // Update UI once at start
        UpdateUI();
    }

    private void Update()
    {
        // Update UI every frame
        UpdateUI();
    }

    // Public method to modify trust from other scripts
    public void AddTrust(float amount)
    {
        // Apply trust change and clamp to bounds
        trust = Mathf.Clamp(trust + amount, minTrust, maxTrust);
    }

    // Public getter for current trust value
    public float CurrentTrust => trust;

    private void UpdateUI()
    {
        // If slider is assigned, update its value
        if (trustSlider != null)
            trustSlider.value = trust;

        // Update trust value text if assigned
        if (trustValueText != null)
            trustValueText.text = Mathf.RoundToInt(trustSlider.value).ToString();

    }
}
