using UnityEngine;
using TMPro;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // Array of all weapons
    private bool[] weaponPickedUp; // Tracks if a weapon is picked up
    public int currentWeaponIndex { get; private set; }

    [Header("God Mode Settings")]
    public bool godModeEnabled = false; // Toggle in the inspector

    [Header("God Mode UI")]
    public TextMeshProUGUI godModeText;
    public string godModeEnabledMessage = "God Mode Enabled"; // Custom message for enabling
    public string godModeDisabledMessage = "God Mode Disabled"; // Custom message for disabling
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public float stayTime = 2f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        weaponPickedUp = new bool[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false); // Disable all weapons at the start
        }

        if (godModeText != null)
        {
            canvasGroup = godModeText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = godModeText.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f; // Start with text invisible
        }
    }

    private void Update()
    {
        if (!IsCurrentWeaponReloading())
        {
            HandleWeaponSwitching();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleGodMode();
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
            if (weapon != null)
            {
                weapon.UpdateWeaponNameUI();
                weapon.UpdateAmmoUI(); // Force UI update when switching weapons

                if (godModeEnabled)
                {
                    weapon.reserveAmmo = 9999; // Set unlimited reserve ammo
                    weapon.UpdateAmmoUI();
                }
            }
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

    public bool HasWeapon(int weaponIndex)
    {
        return weaponIndex >= 0 && weaponIndex < weapons.Length && weaponPickedUp[weaponIndex];
    }

    public void AddAmmoToWeapon(int weaponIndex, int ammoAmount)
    {
        if (HasWeapon(weaponIndex))
        {
            var weapon = weapons[weaponIndex].GetComponent<RaycastWeapon>();
            if (weapon != null)
            {
                weapon.AddAmmo(ammoAmount);
                Debug.Log($"Added {ammoAmount} ammo to {weapons[weaponIndex].name}.");

                if (weaponIndex == currentWeaponIndex)
                {
                    weapon.UpdateAmmoUI();
                }
            }
        }
        else
        {
            Debug.Log($"Cannot add ammo. Weapon {weaponIndex + 1} has not been picked up.");
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

    private void ToggleGodMode()
    {
        godModeEnabled = !godModeEnabled; // Toggle the state
        Debug.Log(godModeEnabled ? "God Mode Activated!" : "God Mode Deactivated!");

        if (godModeEnabled)
        {
            EnableGodMode();
        }
        else
        {
            DisableGodMode();
        }

        // Show the appropriate message
        if (godModeText != null)
        {
            StartCoroutine(ShowGodModeText(godModeEnabled ? godModeEnabledMessage : godModeDisabledMessage));
        }
    }

    private void EnableGodMode()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponPickedUp[i] = true; // Unlock all weapons
        }

        EquipWeapon(0); // Equip the first weapon by default

        foreach (var weaponObj in weapons)
        {
            var weapon = weaponObj.GetComponent<RaycastWeapon>();
            if (weapon != null)
            {
                weapon.reserveAmmo = 9999; // Unlimited ammo
                weapon.UpdateAmmoUI();
            }
        }
    }

    private void DisableGodMode()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponPickedUp[i] = false; // Lock all weapons
        }

        if (currentWeaponIndex >= 0)
        {
            weapons[currentWeaponIndex].SetActive(false);
        }
        currentWeaponIndex = -1;

        Debug.Log("God Mode Off: Weapons reset.");
    }

    private IEnumerator ShowGodModeText(string message)
    {
        godModeText.text = message; // Update message text

        // Fade in
        float timeElapsed = 0f;
        while (timeElapsed < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Stay visible
        yield return new WaitForSeconds(stayTime);

        // Fade out
        timeElapsed = 0f;
        while (timeElapsed < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
