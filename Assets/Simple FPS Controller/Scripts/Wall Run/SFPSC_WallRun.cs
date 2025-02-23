using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFPSC_PlayerMovement))]
public class SFPSC_WallRun : MonoBehaviour
{
    private static Vector3 vecZero = Vector3.zero;

    public Transform startingPosition;

    [Header("Properties")]
    public LayerMask layerMask;
    public float maxDistanceToWall = 1.5f;
    public float constantWallRunForce = 15.0f;
    public float jumpForce = 600.0f;
    public float cameraTiltAngle = 10.0f;
    public float minSpeedWhenAttached = 10.0f;
    public float t1 = 5.0f, multiplier = 4.5f;
    public float jumpWallMultiplier = 0.5f, jumpForwardMultiplier = 0.3f, jumpUpMultiplier = 0.2f;

    [Header("Block times")]
    public float jumpBlockTime = 0.8f;
    public float attachToWallBlockTime = 1.0f;

    [Header("Audio")]
    public AudioSource grindAudioSource;  // Audio Source for grinding sound
    public AudioClip grindClip; // The actual grinding sound file

    public bool IsWallRunning => wallRunning;

    private SFPSC_PlayerMovement pm;
    private Rigidbody rb;

    private RaycastHit hitInfo;
    private bool blocked = false, wallRunning = false;

    private void Start()
    {
        pm = GetComponent<SFPSC_PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        // Setup audio source if not assigned in the Inspector
        if (grindAudioSource == null)
        {
            grindAudioSource = gameObject.AddComponent<AudioSource>();
            grindAudioSource.clip = grindClip;
            grindAudioSource.loop = true;
            grindAudioSource.playOnAwake = false;
            grindAudioSource.volume = 0.5f; // Default volume, adjust as needed
        }
    }

    private void FixedUpdate()
    {
        if (!rb.useGravity || blocked)
            return;

        if (Physics.Raycast(startingPosition.position, transform.right, out hitInfo, maxDistanceToWall, layerMask))
        {
            if (pm.vInput >= 0.5f)
            {
                if (!wallRunning)
                    StartWallRunning();
                AddForces(hitInfo.normal, true);
                return;
            }
        }
        if (Physics.Raycast(startingPosition.position, -transform.right, out hitInfo, maxDistanceToWall, layerMask))
        {
            if (pm.vInput >= 0.5f)
            {
                if (!wallRunning)
                    StartWallRunning();
                AddForces(hitInfo.normal, false);
                return;
            }
        }

        if (wallRunning)
            StopWallRunning();
        gravityForce = vecZero;
    }

    private float t = 0.0f, mag;

    private void StartWallRunning()
    {
        wallRunning = true;
        t = t1;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, t, rb.linearVelocity.z);

        // Start grinding sound
        if (!grindAudioSource.isPlaying)
            grindAudioSource.Play();
    }

    private void StopWallRunning()
    {
        wallRunning = false;
        SFPSC_FPSCamera.cam.rotZ = 0.0f;
        blocked = true;
        Invoke(nameof(UnblockWallRunning), attachToWallBlockTime);

        // Stop grinding sound
        if (grindAudioSource.isPlaying)
            grindAudioSource.Stop();
    }

    private void UnblockWallRunning()
    {
        blocked = false;
    }

    private Vector3 gravityForce;
    private bool isJumpAvailable = true;

private void AddForces(Vector3 wallNormal, bool right)
{
    if (isJumpAvailable && Input.GetKey(SFPSC_KeyManager.Jump))
    {
        rb.AddForce((hitInfo.normal * jumpWallMultiplier + transform.forward * jumpForwardMultiplier + Vector3.up * jumpUpMultiplier).normalized * rb.mass * jumpForce);
        isJumpAvailable = false;
        Invoke(nameof(UnblockJump), jumpBlockTime);
    }

    if (t >= 0.0f)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, t, rb.linearVelocity.z);
        t -= multiplier * Time.fixedDeltaTime;
    }

    mag = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
    if (mag < minSpeedWhenAttached)
        rb.linearVelocity = new Vector3(rb.linearVelocity.x / mag, rb.linearVelocity.y / minSpeedWhenAttached, rb.linearVelocity.z / mag) * minSpeedWhenAttached;

    // Calculate movement direction based on camera's forward and right direction
    Vector3 cameraForward = SFPSC_FPSCamera.cam.transform.forward;
    Vector3 cameraRight = SFPSC_FPSCamera.cam.transform.right;

    // Flatten the forward and right vectors to avoid vertical movement
    cameraForward.y = 0;
    cameraRight.y = 0;

    // Normalize them to maintain consistent speed
    cameraForward.Normalize();
    cameraRight.Normalize();

    // Determine direction based on the camera's position
    Vector3 moveDirection = cameraForward * Input.GetAxis("Vertical") + cameraRight * Input.GetAxis("Horizontal");

    if (moveDirection != Vector3.zero)
    {
        // Apply the direction to the wall-running velocity
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection * mag, Time.deltaTime * 10f);
    }

    SFPSC_FPSCamera.cam.rotZ = right ? cameraTiltAngle : -cameraTiltAngle;

    // Adjust grind sound volume based on speed
    grindAudioSource.volume = Mathf.Clamp(mag / 20f, 0.2f, 1f); // Scale volume with speed
}



    private void UnblockJump()
    {
        isJumpAvailable = true;
    }
}
