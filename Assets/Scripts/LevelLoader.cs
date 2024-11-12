using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(string sceneName)
    {
        // Check if the scene exists in the build settings
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) != -1)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings.");
        }
    }
}
