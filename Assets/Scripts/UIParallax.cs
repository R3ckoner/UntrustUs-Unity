using UnityEngine;

public class UIParallax : MonoBehaviour
{
    public RectTransform uiElement;  // Assign your UI panel here
    public float sensitivity = 50f;  // Increased sensitivity for better movement
    public float smoothTime = 0.1f;  // Smooth transition time

    private Vector2 velocity = Vector2.zero;
    private Vector2 originalPosition;

    void Start()
    {
        // Store the original position so it moves relative to it
        originalPosition = uiElement.anchoredPosition;
    }

    void Update()
    {
        // Get mouse position relative to screen center
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 offset = (mousePos - screenCenter) / screenCenter;  // Normalize to -1 to 1 range

        // Calculate target position
        Vector2 targetPos = originalPosition + (offset * sensitivity);

        // Smoothly move the UI
        uiElement.anchoredPosition = Vector2.SmoothDamp(uiElement.anchoredPosition, targetPos, ref velocity, smoothTime);
    }
}
