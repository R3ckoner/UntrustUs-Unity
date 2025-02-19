using UnityEngine;

public class UIParallax : MonoBehaviour
{
    public RectTransform uiElement;  // Assign your UI panel here
    public float sensitivity = 10f;  // Controls how much the UI moves
    public float smoothTime = 0.1f;  // Smooth transition time

    private Vector2 velocity = Vector2.zero;

    void Update()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Calculate target position
        Vector2 targetPos = new Vector2(-mouseX, -mouseY);

        // Smoothly move the UI
        uiElement.anchoredPosition = Vector2.SmoothDamp(uiElement.anchoredPosition, targetPos, ref velocity, smoothTime);
    }
}
