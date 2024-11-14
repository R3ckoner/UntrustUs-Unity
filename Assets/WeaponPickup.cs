using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int weaponIndex; // Index of the weapon to enable upon pickup

    [Header("UI References")]
    public GameObject normalFaceImage; // Reference to the normal face GameObject
    public GameObject happyFaceImage;  // Reference to the happy face GameObject

    [Header("Audio Settings")]
    public AudioClip pickupSound;      // Sound to play on weapon pickup
    private AudioSource audioSource;   // AudioSource component

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
            WeaponManager weaponManager = other.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.PickupWeapon(weaponIndex);
                Debug.Log($"Picked up Weapon {weaponIndex + 1}!");

                // Play the pickup sound
                if (pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }

                // Trigger the happy face reaction for 1 second
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowHappyFace(1f);
                }

                // Disable visual and collider components
                HidePickup();

                // Destroy the pickup object after the sound finishes
                Destroy(gameObject, pickupSound.length);
            }
        }
    }

    private void HidePickup()
    {
        // Disable renderer(s) to hide the object
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Disable collider(s) to prevent further interactions
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
