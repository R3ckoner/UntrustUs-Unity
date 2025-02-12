using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float speed = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Key Lock Settings")]
    public bool requiresKey = false;
    private bool keyCollected = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip openSound, closeSound, lockedSound;

    private Quaternion closedRotation, openRotation;
    private bool isOpen = false, isAnimating = false, playerInRange = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        Debug.Log($"🚪 Door '{gameObject.name}' initialized. Requires Key: {requiresKey}, Key Collected: {keyCollected}");
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !isAnimating)
        {
            if (!requiresKey || keyCollected)
            {
                StartCoroutine(RotateDoor(isOpen ? closedRotation : openRotation));
                PlaySound(isOpen ? closeSound : openSound);
                isOpen = !isOpen;
                Debug.Log($"🚪 Door '{gameObject.name}' {(isOpen ? "Opened" : "Closed")}.");
            }
            else
            {
                Debug.Log("❌ Door is locked! Find the key.");
                PlaySound(lockedSound);
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
            Debug.Log($"👤 Player entered door '{gameObject.name}' interaction zone.");
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"👤 Player left door '{gameObject.name}' interaction zone.");
            playerInRange = false;
        }
    }

    public void UnlockDoor()
    {
        keyCollected = true;
        Debug.Log($"🔑 Key collected! The door '{gameObject.name}' is now unlocked.");
    }
}
