using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoad : MonoBehaviour
{
    public Button button;  // Reference to the button component
    public string sceneName;  // Name of the scene to load

    void Start()
    {
        // Ensure the button is assigned and add the listener
        if (button != null)
        {
            button.onClick.AddListener(() => LoadScene(sceneName));
        }
        else
        {
            Debug.LogError("Button not assigned in inspector!");
        }
    }

    void LoadScene(string scene)
    {
        if (!string.IsNullOrEmpty(scene))
        {
            SceneManager.LoadScene(scene);
        }
        else
        {
            Debug.LogError("Scene name is empty or null!");
        }
    }
}