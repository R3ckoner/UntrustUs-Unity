using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Door To Unlock")]
    public DoorController doorToUnlock; // Assign the door in the Inspector

    [Header("Enemies to Activate")]
    public GameObject[] enemiesToActivate; // Assign enemies that should become active

    [Header("Audio Settings")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure an AudioSource exists
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"➡️ {other.name} entered the trigger of {gameObject.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player picked up the key!");

            if (doorToUnlock != null)
            {
                doorToUnlock.UnlockDoor();
                Debug.Log($"🔓 Door '{doorToUnlock.gameObject.name}' unlocked!");
            }
            else
            {
                Debug.LogError("❌ No door assigned to this key!");
            }

            // Activate enemies
            if (enemiesToActivate != null && enemiesToActivate.Length > 0)
            {
                foreach (GameObject enemy in enemiesToActivate)
                {
                    if (enemy != null)
                    {
                        enemy.SetActive(true);
                        Debug.Log($"👹 Enemy '{enemy.name}' activated!");
                    }
                }
            }

            // Play pickup sound
            audioSource.PlayOneShot(pickupSound);

            // Instantly hide key (disable mesh + collider)
            GetComponent<Renderer>().enabled = false; // Hide key
            GetComponent<Collider>().enabled = false; // Disable trigger to prevent re-triggering

            // Destroy after sound finishes (or immediately if no sound)
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
        else
        {
            Debug.Log($"🚫 {other.name} is NOT the player.");
        }
    }
}
