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
    public TextMeshProUGUI moneyText; // Reference to TMP money display
    public GameObject sadFaceImage; // Reference to the sad face GameObject

    [Header("Damage Settings")]
    public int damageThreshold = 35; // Damage threshold to trigger sound (can be set in Inspector)
    public AudioClip damageSound; // Sound effect to play when damage exceeds threshold
    private AudioSource audioSource; // Audio source for playing the damage sound

    [Header("Death Settings")]
    public float reloadDelay = 2f; // Delay before reloading the scene

    [Header("Scene Settings")]
    public string gameplaySceneName = "GameplayScene"; // Set this to your gameplay scene name

    [Header("Debug Settings")]
    public bool isGodMode = false; // God Mode toggle

    // Static variables to persist health and money across scenes
    public static int savedHealth = -1; // Initialize to -1 to detect first run
    public static int savedTotalMoney = 0; // Default money to 0

    private int totalMoney;

    void Start()
    {
        // Initialize health
        currentHealth = savedHealth > 0 ? savedHealth : maxHealth;

        // Initialize money
        totalMoney = savedTotalMoney;

        // Update UI
        UpdateHealthUI();
        UpdateMoneyUI();

        // Get the AudioSource component attached to the player
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Toggle God Mode with F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isGodMode = !isGodMode;
            Debug.Log($"🛡️ God Mode: {(isGodMode ? "Enabled" : "Disabled")}");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isGodMode)
        {
            Debug.Log("🛡️ God Mode Active: No Damage Taken!");
            return;
        }

        // Check if damage exceeds threshold
        if (damage > damageThreshold && damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Apply damage and clamp health
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

        // Show or hide sad face based on health threshold
        if (sadFaceImage != null)
        {
            sadFaceImage.SetActive(currentHealth < 40);
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");

        // Reset health after death
        savedHealth = 0;

        // Reload the gameplay scene after a delay
        Invoke(nameof(ReloadGameplayScene), reloadDelay);
    }

    void ReloadGameplayScene()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public void AddMoney(int amount)
    {
        totalMoney += amount;
        savedTotalMoney = totalMoney; // Persist money
        UpdateMoneyUI();
    }

    public void ResetMoney()
    {
        totalMoney = 0;
        savedTotalMoney = 0; // Reset static variable
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{totalMoney}";
        }
        else
        {
            Debug.LogWarning("MoneyText reference is missing!");
        }
    }

    private void OnDisable()
    {
        // Save current health and money when switching scenes
        savedHealth = currentHealth;
        savedTotalMoney = totalMoney;
    }

    // Add this public property to allow access to currentHealth
    public int CurrentHealth
    {
        get { return currentHealth; }
    }
}
