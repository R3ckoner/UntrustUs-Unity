using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int pickupValue = 25; // Amount of money this pickup gives

    [Header("Effects")]
    public AudioSource audioSource;  // AudioSource to be triggered on pickup

    private void Start()
    {
        // Ensure audioSource is assigned. If not, attempt to get one from the object.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Optionally, ensure playOnAwake is off if you don't want the audio to start automatically.
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.AddMoney(pickupValue);  // Use the AddMoney method from PlayerHealth

                // Trigger the AudioSource to play
                if (audioSource != null)
                {
                    audioSource.Play(); // Play the assigned AudioSource
                }

                // Show happy face for 1 second
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowHappyFace(1f);
                }

                Destroy(gameObject); // Destroy pickup after collection
            }
        }
    }
}