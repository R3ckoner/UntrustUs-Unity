using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SFPSC_PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private bool enableMovement = true;

    [Header("Gravity")]
public float gravityMultiplier = 2.0f;  // Default is 1.0 for standard gravity

    [Header("Movement properties")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float changeInStageSpeed = 10.0f;
    public float maximumPlayerSpeed = 150.0f;
    [HideInInspector] public float vInput, hInput;
    public Transform groundChecker;
    public float groundCheckerDist = 0.2f;

    [Header("Jump")]
    public float jumpForce = 500.0f;
    public float jumpCooldown = 1.0f;
    private bool jumpBlocked = false;

    private SFPSC_WallRun wallRun;
    private SFPSC_GrapplingHook grapplingHook;

    [Header("Footstep Sounds")]
    public AudioClip[] defaultFootsteps;
    public float defaultVolume = 1.0f;

    public AudioClip[] woodFootsteps;
    public float woodVolume = 1.0f;

    public AudioClip[] metalFootsteps;
    public float metalVolume = 1.0f;

    public AudioClip[] grassFootsteps;
    public float grassVolume = 1.0f;

    private AudioSource audioSource;
    private bool isPlayingFootstep = false;
    private float footstepInterval = 0.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        TryGetWallRun();
        TryGetGrapplingHook();

        // Initialize AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    public void TryGetWallRun()
    {
        TryGetComponent(out wallRun);
    }

    public void TryGetGrapplingHook()
    {
        TryGetComponent(out grapplingHook);
    }

    private bool isGrounded = false;
    public bool IsGrounded { get { return isGrounded; } }

    private Vector3 inputForce;
    private float prevY;

private void FixedUpdate()
{
    if ((wallRun != null && wallRun.IsWallRunning) || (grapplingHook != null && grapplingHook.IsGrappling))
        isGrounded = false;
    else
    {
        isGrounded = (Mathf.Abs(rb.linearVelocity.y - prevY) < .1f) && (Physics.OverlapSphere(groundChecker.position, groundCheckerDist).Length > 1);
        prevY = rb.linearVelocity.y;
    }

    // Input
    vInput = Input.GetAxisRaw("Vertical");
    hInput = Input.GetAxisRaw("Horizontal");

    // Clamping speed
    rb.linearVelocity = ClampMag(rb.linearVelocity, maximumPlayerSpeed);

    if (!enableMovement)
        return;

    bool isSprinting = Input.GetKey(SFPSC_KeyManager.Run);
    footstepInterval = isSprinting ? 0.3f : 0.5f; // Faster footsteps when sprinting

    inputForce = (transform.forward * vInput + transform.right * hInput).normalized * (isSprinting ? runSpeed : walkSpeed);

    if (isGrounded)
    {
        if (Input.GetButton("Jump") && !jumpBlocked)
        {
            rb.AddForce(-jumpForce * rb.mass * Vector3.down);
            jumpBlocked = true;
            Invoke("UnblockJump", jumpCooldown);
        }

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, inputForce, changeInStageSpeed * Time.fixedDeltaTime);

        if ((vInput != 0 || hInput != 0) && !isPlayingFootstep)
        {
            PlayFootstepSound();
        }
    }
    else
    {
        rb.linearVelocity = ClampSqrMag(rb.linearVelocity + inputForce * Time.fixedDeltaTime, rb.linearVelocity.sqrMagnitude);
    }

    // Apply custom gravity
    if (!isGrounded)
    {
        rb.AddForce(Vector3.up * Physics.gravity.y * gravityMultiplier, ForceMode.Acceleration);
    }
}


    private void PlayFootstepSound()
    {
        (AudioClip[] footstepSet, float volume) = GetSurfaceFootsteps();
        if (footstepSet.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSet.Length);
            audioSource.clip = footstepSet[randomIndex];
            audioSource.volume = volume; // Set volume dynamically
            audioSource.Play();
        }

        isPlayingFootstep = true;
        Invoke("ResetFootstep", footstepInterval);
    }

    private (AudioClip[], float) GetSurfaceFootsteps()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckerDist + 0.1f))
        {
            if (hit.collider.CompareTag("Wood")) return (woodFootsteps, woodVolume);
            if (hit.collider.CompareTag("Metal")) return (metalFootsteps, metalVolume);
            if (hit.collider.CompareTag("Grass")) return (grassFootsteps, grassVolume);
        }
        return (defaultFootsteps, defaultVolume);
    }

    private void ResetFootstep()
    {
        isPlayingFootstep = false;
    }

    private static Vector3 ClampSqrMag(Vector3 vec, float sqrMag)
    {
        if (vec.sqrMagnitude > sqrMag)
            vec = vec.normalized * Mathf.Sqrt(sqrMag);
        return vec;
    }

    private static Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }

    private void UnblockJump()
    {
        jumpBlocked = false;
    }

    public void EnableMovement()
    {
        enableMovement = true;
    }

    public void DisableMovement()
    {
        enableMovement = false;
    }
}
