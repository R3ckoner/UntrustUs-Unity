using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f; // The angle the door should open to
    public float speed = 2f; // Speed of the door opening/closing
    public KeyCode interactKey = KeyCode.E; // Key to open/close the door

    [Header("Interaction Settings")]
    public Collider interactionTrigger; // Assign a BoxCollider for door interaction

    [Header("Key Lock Settings")]
    public bool requiresKey = false; // Toggle for Doom-style key lock
    public GameObject keyObject; // Assign a key item GameObject

    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip openSound; // Sound when door opens
    public AudioClip closeSound; // Sound when door closes
    public AudioClip pickupSound; // Sound when picking up the key
    public AudioClip lockedSound; // Sound when trying to open a locked door

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private bool isAnimating = false;
    private bool playerInRange = false;
    private bool keyCollected = false; // Tracks if the player has picked up the key

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        // If the keyObject is null, consider the door as unlocked
        if (keyObject == null)
        {
            keyCollected = true;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !isAnimating)
        {
            if (!requiresKey || keyCollected) // If key is collected or no key is required
            {
                StartCoroutine(RotateDoor(isOpen ? closedRotation : openRotation));
                PlaySound(isOpen ? closeSound : openSound);
                isOpen = !isOpen;
            }
            else
            {
                PlaySound(lockedSound); // Play locked door sound if key is missing
            }
        }
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isAnimating = true;
        float time = 0f;
        Quaternion startRotation = transform.rotation;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time);
            yield return null;
        }

        isAnimating = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }

        // Check if the player collides with the key object
        if (requiresKey && keyObject != null && other.gameObject == keyObject)
        {
            PickUpKey();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void PickUpKey()
    {
        keyCollected = true;

        // Play pickup sound
        PlaySound(pickupSound);

        // Destroy key object after collection
        if (keyObject != null)
        {
            Destroy(keyObject);
        }
    }
}
