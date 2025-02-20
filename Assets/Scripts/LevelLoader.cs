using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad; // Name of the scene to load
    public bool requireKeyPress = false; // Toggle for requiring "E" key press
    private bool playerInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only the player triggers the level change
        {
            playerInTrigger = true;
            if (!requireKeyPress)
            {
                LoadNextLevel();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    private void Update()
    {
        if (requireKeyPress && playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name not set in LevelChanger script!");
        }
    }
}