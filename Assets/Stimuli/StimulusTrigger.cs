using UnityEngine;

// This script increases fear when the dog touches a stimulus
public class StimulusTrigger : MonoBehaviour
{
    // How much fear to add on contact
    [SerializeField] private float fearAmount = 15f;

    // Reference to the fear system
    [SerializeField] private FearSystem fearSystem;

    private void Start()
    {
        // If fearSystem wasn't set in Inspector, try to find it in the scene
        if (fearSystem == null)
            fearSystem = FindObjectOfType<FearSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the dog
        if (other.gameObject.name == "Dog")
        {
            // Increase fear when the dog hits the stimulus
            fearSystem.AddFear(fearAmount);

            // Optional: disable stimulus after one hit so it won't spam fear
            gameObject.SetActive(false);
        }
    }
}
