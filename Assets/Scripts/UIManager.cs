using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject normalFaceImage;
    public GameObject happyFaceImage;

    public void ShowHappyFace(float duration)
    {
        StartCoroutine(ShowHappyFaceCoroutine(duration));
    }

    private IEnumerator ShowHappyFaceCoroutine(float duration)
    {
        if (normalFaceImage == null || happyFaceImage == null)
        {
            Debug.LogWarning("Face images are not properly assigned.");
            yield break;
        }

        normalFaceImage.SetActive(false);
        happyFaceImage.SetActive(true);

        yield return new WaitForSeconds(duration);

        happyFaceImage.SetActive(false);
        normalFaceImage.SetActive(true);
    }
}
