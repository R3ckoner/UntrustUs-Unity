using UnityEngine;
using UnityEngine.UI;

public class MainMenuCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform creditsPosition; // Target position for the camera when viewing credits
    public float transitionSpeed = 2f; // Speed of the camera movement

    [Header("UI Elements")]
    public GameObject mainMenuObjects; // Assign any GameObject (Main Menu UI or parent)
    public GameObject creditsObjects;  // Assign any GameObject (Credits UI or parent)
    public Button backButton;  // The Back button in the credits UI

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool movingToCredits = false;
    private bool movingToMenu = false;
    private float t = 0;

    void Start()
    {
        // Store the initial camera position and rotation
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Ensure UI starts in the correct state
        if (mainMenuObjects) mainMenuObjects.SetActive(true);
        if (creditsObjects) creditsObjects.SetActive(false);
        if (backButton) backButton.gameObject.SetActive(false); // Start with the Back button hidden

        // Assign the Back button event dynamically
        if (backButton != null)
            backButton.onClick.AddListener(GoBackToMenu);
    }

    void Update()
    {
        if (movingToCredits)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPosition, creditsPosition.position, t);
            transform.rotation = Quaternion.Lerp(startRotation, creditsPosition.rotation, t);

            if (t >= 1f) movingToCredits = false;
        }
        else if (movingToMenu)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(creditsPosition.position, startPosition, t);
            transform.rotation = Quaternion.Lerp(creditsPosition.rotation, startRotation, t);

            if (t >= 1f) movingToMenu = false;
        }
    }

    // Called when Credits button is pressed
    public void ShowCredits()
    {
        if (!movingToCredits)
        {
            t = 0;
            movingToCredits = true;
            movingToMenu = false;

            if (mainMenuObjects) mainMenuObjects.SetActive(false);
            if (creditsObjects) creditsObjects.SetActive(true);
            if (backButton) backButton.gameObject.SetActive(true); // Enable the Back button
        }
    }

    // Called when Back button is pressed
    public void GoBackToMenu()
    {
        if (!movingToMenu)
        {
            t = 0;
            movingToMenu = true;
            movingToCredits = false;

            if (creditsObjects) creditsObjects.SetActive(false);
            if (mainMenuObjects) mainMenuObjects.SetActive(true);
            if (backButton) backButton.gameObject.SetActive(false); // Disable the Back button
        }
    }
}
