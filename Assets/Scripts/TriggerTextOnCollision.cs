using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerTextOnCollision : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI tutorialText;  
    public string tutorialMessage = "This is your tutorial advice!";  
    public float fadeInTime = 1f;  
    public float fadeOutTime = 1f;  
    public float stayTime = 3f;  

    [Header("Repeatable Toggle")]
    public bool isRepeatable = true;  

    private CanvasGroup canvasGroup;
    private bool hasTriggered = false;  
    private Coroutine activeCoroutine = null; // Track the active coroutine

    private void Start()
    {
        canvasGroup = tutorialText.GetComponent<CanvasGroup>() ?? tutorialText.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isRepeatable || !hasTriggered)
            {
                tutorialText.text = tutorialMessage;

                // Stop any running coroutine before starting a new one
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }

                activeCoroutine = StartCoroutine(ShowTextEffect());
                hasTriggered = true;
            }
        }
    }

    private IEnumerator ShowTextEffect()
    {
        yield return FadeIn();
        yield return new WaitForSeconds(stayTime);
        yield return FadeOut();

        if (isRepeatable)
        {
            hasTriggered = false;
        }

        activeCoroutine = null; // Reset active coroutine reference
    }

    private IEnumerator FadeIn()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeInTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeOutTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
