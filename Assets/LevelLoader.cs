using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad; // Name of the scene to load

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only the player triggers the level change
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