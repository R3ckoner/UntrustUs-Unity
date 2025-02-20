using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;

    // New field for NPC portrait
    public Texture NpcPortrait;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SpeakTo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
            DialogueManager.Instance.EndDialogue();
        }
    }

    public void SpeakTo()
    {
        if (Dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode, NpcPortrait);
        }
        else
        {
            Debug.LogWarning("Dialogue is not assigned for this actor.");
        }
    }
}