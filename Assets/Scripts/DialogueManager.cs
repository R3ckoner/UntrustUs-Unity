using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // UI references
    public GameObject DialogueParent;
    public TextMeshProUGUI DialogTitleText, DialogBodyText;
    public GameObject responseButtonPrefab;
    public Transform responseButtonContainer;

    // New portrait image reference
    public RawImage NpcPortrait;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        HideDialogue();
    }

    public void StartDialogue(string title, DialogueNode node, Texture portrait)
    {
        if (node == null)
        {
            Debug.LogError("DialogueNode is null. Cannot start dialogue.");
            return;
        }

        ShowDialogue();
        DialogTitleText.text = title;
        DialogBodyText.text = node.dialogueText;

        // Set NPC portrait
        if (NpcPortrait != null && portrait != null)
        {
            NpcPortrait.texture = portrait;
            NpcPortrait.gameObject.SetActive(true);
        }
        else if (NpcPortrait != null)
        {
            NpcPortrait.gameObject.SetActive(false); // Hide if no portrait
        }

        // Clear existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create response buttons
        foreach (DialogueResponse response in node.responses)
        {
            if (responseButtonPrefab == null)
            {
                Debug.LogError("Response button prefab is missing.");
                continue;
            }

            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = response.responseText;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component is missing in the response button prefab.");
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                DialogueResponse capturedResponse = response;
                button.onClick.AddListener(() => SelectResponse(capturedResponse, title));
            }
            else
            {
                Debug.LogError("Button component is missing in the response button prefab.");
            }
        }
    }

    public void SelectResponse(DialogueResponse response, string title)
    {
        if (response == null)
        {
            Debug.LogError("Response is null. Cannot select response.");
            return;
        }

        if (response.nextNode != null)
        {
            StartDialogue(title, response.nextNode, NpcPortrait.texture);
        }
        else
        {
            Debug.Log("End of dialogue reached.");
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        HideDialogue();
    }

    public void HideDialogue()
    {
        DialogueParent.SetActive(false);
        SetCursorVisibility(false);
    }

    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
        SetCursorVisibility(true);
    }

    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }

    private void SetCursorVisibility(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
