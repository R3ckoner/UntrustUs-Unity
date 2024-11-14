using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons;
    private bool[] weaponPickedUp;
    private int currentWeaponIndex = -1;

    private void Start()
    {
        weaponPickedUp = new bool[weapons.Length];
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
            if (currentWeaponIndex >= 0)
            {
                weapons[currentWeaponIndex].SetActive(false);
            }

            weapons[weaponIndex].SetActive(true);
            currentWeaponIndex = weaponIndex;
            Debug.Log($"Equipped Weapon {weaponIndex + 1}");
        }
    }

    public void PickupWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            weaponPickedUp[weaponIndex] = true;
            EquipWeapon(weaponIndex);
        }
    }

    private void HandleWeaponSwitching()
    {
        if (currentWeaponIndex >= 0 && weapons[currentWeaponIndex].GetComponent<RaycastWeapon>().IsReloading)
        {
            Debug.Log("Cannot switch weapons while reloading.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponPickedUp[0]) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponPickedUp[1]) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponPickedUp[2]) EquipWeapon(2);
    }
}