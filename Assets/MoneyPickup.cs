using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int pickupValue = 25; // Amount of money this pickup gives

    [Header("Effects")]
    public AudioClip pickupSound;  // Optional: Sound clip that plays on pickup
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure there's an AudioSource component on the GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false; // Ensure sound doesn't play automatically
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerMoneyManager = other.GetComponent<PlayerMoneyManager>();
            if (playerMoneyManager != null)
            {
                playerMoneyManager.AddMoney(pickupValue);

                // Play pickup sound if available
                if (pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
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