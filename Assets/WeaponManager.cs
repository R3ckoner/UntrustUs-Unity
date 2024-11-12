using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // Array to hold all player weapons
    private bool[] weaponPickedUp; // Tracks which weapons have been picked up
    private int currentWeaponIndex = -1; // No weapon equipped at start

    private void Start()
    {
        weaponPickedUp = new bool[weapons.Length];

        // Ensure all weapons are deactivated initially
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
    }

    private void Update()
    {
        HandleWeaponSwitching();
    }

    public void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length && weaponPickedUp[weaponIndex])
        {
            // Deactivate the current weapon if any
            if (currentWeaponIndex >= 0)
            {
                weapons[currentWeaponIndex].SetActive(false);
            }

            // Activate the new weapon
            weapons[weaponIndex].SetActive(true);
            currentWeaponIndex = weaponIndex;
            Debug.Log($"Equipped Weapon {weaponIndex + 1}");
        }
        else
        {
            Debug.LogError($"Cannot equip weapon {weaponIndex + 1}: either out of bounds or not picked up yet.");
        }
    }

    public void PickupWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            weaponPickedUp[weaponIndex] = true; // Mark weapon as picked up
            EquipWeapon(weaponIndex); // Automatically equip it upon pickup
        }
        else
        {
            Debug.LogError("Invalid weapon index.");
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponPickedUp[0])
            EquipWeapon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponPickedUp[1])
            EquipWeapon(1);

        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponPickedUp[2])
            EquipWeapon(2);
    }
}
