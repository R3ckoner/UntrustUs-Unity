using UnityEngine;
using TMPro;

public class RaycastWeapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
    public float fireRate = 0.1f;
    public bool isFullAuto = true;
    public int magazineSize = 30;
    public int reserveAmmo = 90;
    public float range = 100f;
    public float damage = 10f;

    [Header("UI Elements")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI bagText;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;

    [Header("Recoil Settings")]
    public Transform weaponTransform;
    public Vector3 recoilKickback = new Vector3(0f, 0.2f, -0.3f);
    public Vector3 recoilRotation = new Vector3(-2f, 1f, 0f);
    public float recoilSmoothTime = 0.1f;
    public float recoilResetSpeed = 2f;

    [Header("Audio")]
    public AudioSource fireSound;
    public AudioSource reloadSound;

    private int currentAmmo;
    private float nextFireTime = 0f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Vector3 currentRecoilPosition;
    private Vector3 recoilPositionVelocity = Vector3.zero;

    private Quaternion currentRecoilRotation;
    private Quaternion recoilRotationVelocity;

    void Start()
    {
        currentAmmo = magazineSize;
        originalPosition = weaponTransform.localPosition;
        originalRotation = weaponTransform.localRotation;
        UpdateAmmoUI();
    }

    void Update()
    {
        if ((isFullAuto && Input.GetButton("Fire1") || Input.GetButtonDown("Fire1")) && Time.time >= nextFireTime)
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        ResetRecoil();
    }

    void Fire()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            nextFireTime = Time.time + fireRate;

            // Play fire sound
            if (fireSound != null)
            {
                fireSound.Play();
            }

            // Raycasting
            RaycastHit hit;
            if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range))
            {
                Debug.Log($"Hit: {hit.collider.name}");
                // Apply damage logic if applicable
            }

            // Muzzle Flash
            ShowMuzzleFlash();

            // Apply Recoil
            ApplyRecoil();

            UpdateAmmoUI();
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }

    void Reload()
    {
        int ammoNeeded = magazineSize - currentAmmo;
        if (reserveAmmo >= ammoNeeded)
        {
            reserveAmmo -= ammoNeeded;
            currentAmmo = magazineSize;
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        // Play reload sound
        if (reloadSound != null)
        {
            reloadSound.Play();
        }

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        magText.text = currentAmmo.ToString();
        bagText.text = reserveAmmo.ToString();
    }

    void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
            Destroy(muzzleFlash, 0.1f);
        }
    }

    void ApplyRecoil()
    {
        if (weaponTransform != null)
        {
            currentRecoilPosition += recoilKickback;
            currentRecoilRotation *= Quaternion.Euler(recoilRotation);
        }
    }

    void ResetRecoil()
    {
        if (weaponTransform != null)
        {
            currentRecoilPosition = Vector3.SmoothDamp(
                currentRecoilPosition,
                Vector3.zero,
                ref recoilPositionVelocity,
                recoilSmoothTime
            );

            currentRecoilRotation = Quaternion.Slerp(
                currentRecoilRotation,
                Quaternion.identity,
                Time.deltaTime * recoilResetSpeed
            );

            weaponTransform.localPosition = Vector3.Lerp(
                weaponTransform.localPosition,
                originalPosition + currentRecoilPosition,
                Time.deltaTime * recoilResetSpeed
            );

            weaponTransform.localRotation = Quaternion.Slerp(
                weaponTransform.localRotation,
                originalRotation * currentRecoilRotation,
                Time.deltaTime * recoilResetSpeed
            );
        }
    }
}
