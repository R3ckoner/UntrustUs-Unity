using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // Array of all weapons
    private bool[] weaponPickedUp; // Tracks if a weapon is picked up
    private int currentWeaponIndex = -1;

    private void Start()
    {
        weaponPickedUp = new bool[weapons.Length];
        
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false); // Disable all weapons at the start
        }
    }

    private void Update()
    {
        if (!IsCurrentWeaponReloading())
        {
            HandleWeaponSwitching();
        }
    }

    public void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length && weaponPickedUp[weaponIndex])
        {
            if (currentWeaponIndex >= 0)
            {
                weapons[currentWeaponIndex].SetActive(false);
            }

            weapons[weaponIndex].SetActive(true);
            currentWeaponIndex = weaponIndex;

            var weapon = weapons[weaponIndex].GetComponent<RaycastWeapon>();
            weapon?.UpdateWeaponNameUI();
        }
    }

    public void PickupWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            weaponPickedUp[weaponIndex] = true; // Mark as picked up
            EquipWeapon(weaponIndex); // Equip the picked-up weapon
            Debug.Log($"Picked up weapon: {weapons[weaponIndex].name}");
        }
        else
        {
            Debug.LogError("Invalid weapon index.");
        }
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponPickedUp[0]) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponPickedUp[1]) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponPickedUp[2]) EquipWeapon(2);
    }

    public bool IsCurrentWeaponReloading()
    {
        if (currentWeaponIndex >= 0)
        {
            var currentWeapon = weapons[currentWeaponIndex].GetComponent<RaycastWeapon>();
            if (currentWeapon != null)
            {
                return currentWeapon.IsReloading;
            }
        }
        return false;
    }
}
