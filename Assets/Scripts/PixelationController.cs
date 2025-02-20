using UnityEngine;
using UnityEngine.UI;

public class PixelationController : MonoBehaviour
{
    public Material pixelationMaterial; // Assign your shader material in Inspector
    public Slider pixelationSlider; // Assign the UI slider in Inspector

    void Start()
    {
        if (pixelationSlider != null)
        {
            // Load saved pixelation level or set default
            pixelationSlider.value = PlayerPrefs.GetFloat("PixelScale", 1f);
            pixelationSlider.onValueChanged.AddListener(SetPixelScale);
        }
    }

    public void SetPixelScale(float value)
    {
        pixelationMaterial.SetFloat("_PixelScale", value);
        PlayerPrefs.SetFloat("PixelScale", value);
        PlayerPrefs.Save(); // Save setting
    }
}

