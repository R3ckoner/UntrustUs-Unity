using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Maximum health
    private int currentHealth;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText; // Reference to TMP health display

    [Header("Death Settings")]
    public float reloadDelay = 2f; // Delay before reloading the scene

    [Header("Scene Settings")]
    public string gameplaySceneName = "GameplayScene"; // Set this to your gameplay scene name

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");

        // Reload the gameplay scene after a delay
        Invoke(nameof(ReloadGameplayScene), reloadDelay);
    }

    void ReloadGameplayScene()
    {
        // Reload the current active scene or specified gameplay scene
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }
}
