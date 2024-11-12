using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;       // How far back the weapon moves
    public float recoilRotation = 5f;       // How much the weapon rotates back
    public float recoilSpeed = 10f;         // How quickly the weapon moves back during recoil
    public float returnSpeed = 5f;          // How smoothly the weapon returns to its original position

    [Header("Firing Settings")]
    public float fireRate = 0.2f;           // Time between shots
    private float nextFireTime = 0f;        // Time when the next shot can be fired

    private Vector3 initialPosition;        // Weapon's starting position
    private Quaternion initialRotation;     // Weapon's starting rotation

    private Vector3 currentRecoilPosition;  // Current recoil offset position
    private Vector3 currentRecoilRotation;  // Current recoil rotation

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        HandleInput();
        HandleRecoil();
    }

    private void HandleInput()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        nextFireTime = Time.time + fireRate;

        // Apply recoil position and rotation
        currentRecoilPosition = new Vector3(0, Random.Range(-recoilAmount / 2, recoilAmount / 2), -recoilAmount);
        currentRecoilRotation = new Vector3(-recoilRotation, Random.Range(-recoilRotation / 2, recoilRotation / 2), 0);

        // Add your shooting logic here (e.g., instantiate bullets, play sound)
        Debug.Log("Fired!");
    }

    private void HandleRecoil()
    {
        // Apply recoil movement and rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + currentRecoilPosition, Time.deltaTime * recoilSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(initialRotation.eulerAngles + currentRecoilRotation), Time.deltaTime * recoilSpeed);

        // Smoothly return to the initial position and rotation
        currentRecoilPosition = Vector3.Lerp(currentRecoilPosition, Vector3.zero, Time.deltaTime * returnSpeed);
        currentRecoilRotation = Vector3.Lerp(currentRecoilRotation, Vector3.zero, Time.deltaTime * returnSpeed);
    }
}
