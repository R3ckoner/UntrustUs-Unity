using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light flickerLight; // Reference to the Light component
    public float minIntensity = 0.5f; // Minimum light intensity
    public float maxIntensity = 2.0f; // Maximum light intensity
    public float flickerSpeed = 0.1f; // Speed of flickering
    public float smoothness = 0.5f; // How smooth the transition is

    private float targetIntensity;
    private float currentIntensity;

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();
        
        currentIntensity = flickerLight.intensity;
        targetIntensity = currentIntensity;
    }

    void Update()
    {
        if (Mathf.Abs(currentIntensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
        
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, smoothness * Time.deltaTime / flickerSpeed);
        flickerLight.intensity = currentIntensity;
    }
}
