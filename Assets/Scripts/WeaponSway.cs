using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.02f; // How much the weapon sways
    public float swaySmoothness = 6f; // How smooth the sway is
    public float maxSway = 0.03f; // Maximum amount of sway

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        HandleSway();
    }

    private void HandleSway()
    {
        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        // Calculate sway amount based on input
        float swayX = Mathf.Clamp(-mouseX * swayAmount, -maxSway, maxSway);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -maxSway, maxSway);

        // Create the target position for the weapon sway
        Vector3 targetPosition = new Vector3(swayX, swayY, 0f);

        // Smoothly move towards the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + targetPosition, Time.deltaTime * swaySmoothness);
    }
}
