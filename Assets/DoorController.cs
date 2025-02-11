using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f; // The angle the door should open to
    public float speed = 2f; // Speed of the door opening/closing
    public KeyCode interactKey = KeyCode.E; // Key to open/close the door
    public Collider interactionTrigger; // Assign a BoxCollider in the Inspector
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip openSound; // Sound when door opens
    public AudioClip closeSound; // Sound when door closes

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private bool isAnimating = false;
    private bool playerInRange = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !isAnimating)
        {
            StartCoroutine(RotateDoor(isOpen ? closedRotation : openRotation));
            PlaySound(isOpen ? closeSound : openSound);
            isOpen = !isOpen;
        }
    }

    System.Collections.IEnumerator RotateDoor(Quaternion targetRotation)
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
