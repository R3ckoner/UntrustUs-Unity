using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Door To Unlock")]
    public DoorController doorToUnlock; // Assign the specific door in the Inspector

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

            if (pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // Destroy key immediately or after sound plays
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
        else
        {
            Debug.Log($"🚫 {other.name} is NOT the player.");
        }
    }
}
