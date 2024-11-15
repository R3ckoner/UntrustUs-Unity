using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    private bool isPlayerInRange = false;

    private void Update()
    {
        // Trigger dialogue only when the player presses E and is in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SpeakTo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the Player tag is correct
        {
            isPlayerInRange = true;
            Debug.Log("Player in range. Press E to interact.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player out of range.");
            DialogueManager.Instance.EndDialogue(); // Optionally end dialogue on exit
        }
    }

    public void SpeakTo()
    {
        if (Dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
        }
        else
        {
            Debug.LogWarning("Dialogue is not assigned for this actor.");
        }
    }
}