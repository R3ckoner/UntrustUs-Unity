using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public Image blackScreen; // UI Image covering the screen (black)
    public TMP_Text levelText; // TextMeshPro UI text for level name or message
    public float fadeDuration = 2f; // Duration of fade-in
    public float textDisplayTime = 2f; // How long the text stays visible

    void Start()
    {
        StartCoroutine(PlayTransition());
    }

    IEnumerator PlayTransition()
    {
        // Ensure black screen and text are fully visible at start
        blackScreen.color = new Color(0, 0, 0, 1);
        levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 1);

        // Wait while displaying the text
        yield return new WaitForSeconds(textDisplayTime);

        // Fade out text
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration / 2)
        {
            levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 1 - (elapsedTime / (fadeDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 0); // Fully transparent

        // Fade in from black
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            blackScreen.color = new Color(0, 0, 0, 1 - (elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0); // Fully transparent
    }
}