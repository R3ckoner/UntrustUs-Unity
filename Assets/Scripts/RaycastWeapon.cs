using UnityEngine;
using TMPro;
using System.Collections;

public class RaycastWeapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
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

    public bool IsReloading => isReloading; // Public property to expose reloading state

    private void Start()
    {
        currentAmmo = magazineSize;
        originalPosition = weaponTransform.localPosition;
        originalRotation = weaponTransform.localRotation;
        UpdateAmmoUI();
    }

    private void Update()
    {
        if (isReloading) return;

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
            var turret = hit.collider.GetComponent<TurretController>();
            turret?.TakeDamage((int)damage);
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
        isReloading = true;
        reloadSound?.Play();

        Vector3 targetPosition = originalPosition + reloadPositionOffset;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(reloadRotationOffset);

        yield return MoveWeapon(targetPosition, targetRotation, reloadTime * 0.5f);
        yield return new WaitForSeconds(reloadTime * 0.5f);

        int ammoToReload = Mathf.Min(magazineSize - currentAmmo, reserveAmmo);
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI();
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

    private void UpdateAmmoUI()
    {
        magText.text = currentAmmo.ToString();
        bagText.text = reserveAmmo.ToString();
    }

    private void ShowMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) return;

        var muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
        Destroy(muzzleFlash, 0.1f);
    }

    private void ApplyRecoil()
    {
        currentRecoilPosition += recoilKickback;
        currentRecoilRotation *= Quaternion.Euler(recoilRotation);
    }

    private void ResetRecoil()
    {
        currentRecoilPosition = Vector3.SmoothDamp(currentRecoilPosition, Vector3.zero, ref recoilPositionVelocity, recoilSmoothTime);
        currentRecoilRotation = Quaternion.Slerp(currentRecoilRotation, Quaternion.identity, Time.deltaTime * recoilResetSpeed);

        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, originalPosition + currentRecoilPosition, Time.deltaTime * recoilResetSpeed);
        weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, originalRotation * currentRecoilRotation, Time.deltaTime * recoilResetSpeed);
    }
}
