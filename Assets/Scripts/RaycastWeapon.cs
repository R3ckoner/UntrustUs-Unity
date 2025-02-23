using UnityEngine;
using TMPro;
using System.Collections;

public class RaycastWeapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public string weaponName = "Default Weapon"; // Name of the weapon
    public float fireRate = 0.1f;
    public bool isFullAuto = true;
    public int magazineSize = 30;
    public int reserveAmmo = 90;
    public float range = 100f;
    public float damage = 10f;
    public float reloadTime = 1.5f;

    [Header("UI Elements")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI bagText;
    public TextMeshProUGUI weaponNameText; // Weapon name display

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;

    [Header("Recoil Settings")]
    public Transform weaponTransform;
    public Vector3 recoilKickback = new Vector3(0f, 0.2f, -0.3f);
    public Vector3 recoilRotation = new Vector3(-2f, 1f, 0f);
    public float recoilSmoothTime = 0.1f;
    public float recoilResetSpeed = 2f;

    [Header("Reload Animation Settings")]
    public Vector3 reloadPositionOffset = new Vector3(0, -0.5f, 0.3f);
    public Vector3 reloadRotationOffset = new Vector3(30f, 0f, 0f);
    public float reloadSmoothTime = 0.2f;
    [Header("Custom Reload Animation")]
    public bool useCustomReload = false; // Toggle in Inspector
    public Animator weaponAnimator; // Animator for custom reload
    public string reloadAnimationName = "Reload"; // Default animation name


    [Header("Audio")]
    public AudioSource fireSound;
    public AudioSource reloadSound;

    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isReloading = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Vector3 currentRecoilPosition;
    private Vector3 recoilPositionVelocity = Vector3.zero;

    private Quaternion currentRecoilRotation;
    private Quaternion recoilRotationVelocity;

    public bool IsReloading => isReloading;

    // Reference to the PauseMenu
    public PauseMenu pauseMenu;

    private void Start()
    {
        currentAmmo = magazineSize;
        originalPosition = weaponTransform.localPosition;
        originalRotation = weaponTransform.localRotation;
        UpdateAmmoUI();
        UpdateWeaponNameUI();
        weaponAnimator.enabled = false;
        
        // Ensure PauseMenu is assigned in the Inspector or find it
        if (pauseMenu == null)
        {
            pauseMenu = FindObjectOfType<PauseMenu>();
        }
    }

    private void Update()
    {
        if (isReloading || (pauseMenu != null && pauseMenu.isPaused)) return; // Skip input if paused

        HandleFiring();
        HandleReloading();
        ResetRecoil();
    }

    private void HandleFiring()
    {
        if ((isFullAuto && Input.GetButton("Fire1") || Input.GetButtonDown("Fire1")) && Time.time >= nextFireTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        fireSound?.Play();

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log($"Hit: {hit.collider.name}");

            // ✅ Apply damage to turrets
            var turret = hit.collider.GetComponent<TurretController>();
            if (turret != null)
            {
                turret.TakeDamage((int)damage);
            }

            // ✅ Apply damage to enemies
            var enemy = hit.collider.GetComponent<EnemyFollow>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
            }
        }
        else
        {
            Debug.Log("Missed!");
        }

        ShowMuzzleFlash();
        ApplyRecoil();
        UpdateAmmoUI();
    }

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (isReloading) yield break;

        isReloading = true;
        reloadSound?.Play();

        if (weaponAnimator != null && weaponAnimator.runtimeAnimatorController != null)
        {
            float animLength = GetAnimationLength("Reload");

            if (animLength > 0f) // ✅ Only play if animation exists
            {
                weaponAnimator.enabled = true;
                weaponAnimator.Play("Reload");
                yield return new WaitForSeconds(animLength);
                weaponAnimator.enabled = false;
            }
            else
            {
                yield return PlayDefaultReloadAnimation(); // ✅ Fallback if animation doesn't exist
            }
        }
        else
        {
            yield return PlayDefaultReloadAnimation(); // ✅ Fallback if no Animator assigned
        }

        // Reload logic
        int ammoToReload = Mathf.Min(magazineSize - currentAmmo, reserveAmmo);
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI();
    }

    private IEnumerator PlayDefaultReloadAnimation()
    {
        Vector3 targetPosition = originalPosition + reloadPositionOffset;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(reloadRotationOffset);

        yield return MoveWeapon(targetPosition, targetRotation, reloadSmoothTime);
        yield return new WaitForSeconds(reloadTime - (2 * reloadSmoothTime));
        yield return MoveWeapon(originalPosition, originalRotation, reloadSmoothTime);
    }

    private float GetAnimationLength(string animationName)
    {
        if (weaponAnimator == null) return reloadTime; // Fallback to default reload time

        AnimationClip[] clips = weaponAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return reloadTime; // If animation not found, use default reload time
    }

    private IEnumerator MoveWeapon(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetPosition, reloadSmoothTime);
            weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, targetRotation, reloadSmoothTime);
            yield return null;
        }
    }

    public void UpdateAmmoUI()
    {
        magText.text = currentAmmo.ToString();
        bagText.text = reserveAmmo.ToString();
    }

    public void UpdateWeaponNameUI()
    {
        if (weaponNameText != null)
        {
            weaponNameText.text = weaponName;
        }
    }

    private void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) return;

        var muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
        Destroy(muzzleFlash, 0.1f);
    }

    private void ApplyRecoil()
    {
        // Apply recoil after firing
        currentRecoilPosition += recoilKickback;
        currentRecoilRotation *= Quaternion.Euler(recoilRotation);
    }

    private void ResetRecoil()
    {
        // Smoothly return weapon to original position after recoil
        currentRecoilPosition = Vector3.SmoothDamp(currentRecoilPosition, Vector3.zero, ref recoilPositionVelocity, recoilSmoothTime);
        currentRecoilRotation = Quaternion.Slerp(currentRecoilRotation, Quaternion.identity, Time.deltaTime * recoilResetSpeed);

        // Apply recoil movement *only* if not reloading
        if (!isReloading)
        {
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, originalPosition + currentRecoilPosition, Time.deltaTime * recoilResetSpeed);
            weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, originalRotation * currentRecoilRotation, Time.deltaTime * recoilResetSpeed);
        }
    }

    // Method to add ammo to the weapon
    public void AddAmmo(int amount)
    {
        reserveAmmo += amount; // Adds ammo to the reserve

        // Only update UI if this weapon is currently equipped
        WeaponManager weaponManager = FindObjectOfType<WeaponManager>();
        if (weaponManager != null && weaponManager.currentWeaponIndex >= 0 &&
            weaponManager.weapons[weaponManager.currentWeaponIndex] == gameObject)
        {
            UpdateAmmoUI(); 
        }
    }
}
