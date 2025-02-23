using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign PauseMenu panel in Inspector
    public Slider volumeSlider; // Assign Slider in Inspector
    public Slider sensitivitySlider;  // Assign Slider in Inspector for mouse sensitivity
    public SFPSC_FPSCamera fpsCamera;  // Reference to FPS Camera script
    public bool isPaused = false;

    void Start()
    {
        // Load saved volume level or set default
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volumeSlider.value;
        
        // Add listener for volume changes
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Initialize sensitivity slider based on current value
        if (sensitivitySlider != null && fpsCamera != null)
        {
            sensitivitySlider.value = fpsCamera.sensitivity;
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        isPaused = true;

        Cursor.visible = true; // Show cursor
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unpause the game
        isPaused = false;

        Cursor.visible = false; // Hide cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor back to center
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset time before switching scenes
        SceneManager.LoadScene("menu"); // Load the main menu scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure time is normal before quitting
        Application.Quit(); // Quit game (only works in build)
        Debug.Log("Game Quit!"); // Debug for editor mode
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save(); // Save volume setting
    }

    public void UpdateSensitivity(float value)
    {
        if (fpsCamera != null)
        {
            fpsCamera.SetSensitivity(value);  // Update sensitivity in the camera script
        }
    }
}
