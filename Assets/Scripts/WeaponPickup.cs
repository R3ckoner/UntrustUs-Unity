using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Pickup Type")]
    public bool isWeaponPickup = true; // Toggle between weapon or ammo pickup

    [Header("Weapon Settings")]
    public int weaponIndex; // Index of the weapon to enable upon pickup

    [Header("Ammo Settings")]
    public int ammoAmount = 30; // Amount of ammo to give (if it's an ammo pickup)

    [Header("UI References")]
    public GameObject normalFaceImage; // Reference to the normal face UI
    public GameObject happyFaceImage;  // Reference to the happy face UI

    [Header("Audio Settings")]
    public AudioClip pickupSound;      // Sound to play on pickup
    private AudioSource audioSource;   // AudioSource component

    private void Start()
    {
        // Ensure there's an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false; // Prevent automatic playback
    }

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        WeaponManager weaponManager = other.GetComponent<WeaponManager>();
        if (weaponManager != null && !weaponManager.IsCurrentWeaponReloading())
        {
            if (isWeaponPickup)
            {
                // Picking up a new weapon
                weaponManager.PickupWeapon(weaponIndex);
                Debug.Log($"Picked up Weapon {weaponIndex + 1}!");
                ProcessPickup();
            }
            else
            {
                // Check if the player already has the weapon before allowing ammo pickup
                if (weaponManager.HasWeapon(weaponIndex))
                {
                    weaponManager.AddAmmoToWeapon(weaponIndex, ammoAmount);
                    Debug.Log($"Picked up {ammoAmount} ammo for Weapon {weaponIndex + 1}!");
                    ProcessPickup();
                }
                else
                {
                    Debug.Log($"Cannot pick up ammo! You don't have Weapon {weaponIndex + 1} yet.");
                }
            }
        }
        else
        {
            Debug.Log("Cannot pick up while reloading.");
        }
    }
}


    private void ProcessPickup()
    {
        // Play pickup sound
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Show happy face UI for 1 second
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowHappyFace(1f);
        }

        // Hide visual and collider components
        HidePickup();

        // Destroy pickup object after sound finishes
        Destroy(gameObject, pickupSound.length);
    }

    private void HidePickup()
    {
        // Disable renderers to make the object invisible
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Disable colliders to prevent further interactions
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
