using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public int healthAmount = 25; // Amount of health restored upon pickup

    [Header("Audio Settings")]
    public AudioClip pickupSound; // Sound to play when health is picked up
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure there's an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false; // Prevent sound from playing automatically
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount); // Heal the player

                // Play pickup sound if available
                if (pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowHappyFace(1f);
                }

                // Destroy the health pickup immediately
                Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
            }
        }
    }
}