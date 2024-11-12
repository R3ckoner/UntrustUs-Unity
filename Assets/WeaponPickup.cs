using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public int weaponIndex; // The index of the weapon to enable upon pickup

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponManager weaponManager = other.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.PickupWeapon(weaponIndex);
                Debug.Log($"Picked up Weapon {weaponIndex + 1}!");
                Destroy(gameObject); // Destroy the pickup after collection
            }
        }
    }
}