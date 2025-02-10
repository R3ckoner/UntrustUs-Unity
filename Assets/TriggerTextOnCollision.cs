using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerTextOnCollision : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI tutorialText;  // Reference to the TextMeshProUGUI component
    public string tutorialMessage = "This is your tutorial advice!";  // The message to display for this pickup
    public float fadeInTime = 1f;  // Time to fade in
    public float fadeOutTime = 1f;  // Time to fade out
    public float stayTime = 3f; // How long to display the text before fading out

    [Header("Repeatable Toggle")]
    public bool isRepeatable = true;  // Toggle to make the effect repeatable or not

    private CanvasGroup canvasGroup;
    private bool hasTriggered = false;  // Flag to track if the text has already been shown

    private void Start()
    {
        // Ensure we have a CanvasGroup for fading in/out.
        canvasGroup = tutorialText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = tutorialText.gameObject.AddComponent<CanvasGroup>();
        }

        // Set initial alpha to 0 for the text to be invisible initially
        canvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger zone and if the effect is repeatable or not
        if (other.CompareTag("Player"))
        {
            if (isRepeatable || !hasTriggered)  // If repeatable or hasn't triggered yet
            {
                // Set the tutorial message dynamically each time
                tutorialText.text = tutorialMessage;

                // Start the text fade-in/fade-out effect
                StartCoroutine(ShowTextEffect());
                hasTriggered = true;  // Mark the text as triggered
            }
        }
    }

    private IEnumerator ShowTextEffect()
    {
        // Fade in the text
        yield return FadeIn();

        // Stay visible for the specified time
        yield return new WaitForSeconds(stayTime);

        // Fade out the text
        yield return FadeOut();

        if (isRepeatable)
        {
            hasTriggered = false;  // Allow the text to trigger again if it's repeatable
        }
    }

    private IEnumerator FadeIn()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeInTime)
        {
            // Gradually increase the alpha value to fade in
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;  // Ensure the alpha is fully 1 after fading in
    }

    private IEnumerator FadeOut()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeOutTime)
        {
            // Gradually decrease the alpha value to fade out
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;  // Ensure the alpha is fully 0 after fading out
    }
}
