using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float followDistance = 5f;
    public float stopDistance = 0.025f;

    [Header("Audio Settings")]
    public AudioSource deathAudioSource;
    public AudioClip triggerSound, deathSound;
    [Range(0f, 1f)] public float deathSoundVolume = 1f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Rewards")]
    public int rewardAmount = 50;

    [Header("Gib Settings")]
    public GameObject gibPrefab;
    public Vector3 gibSize = Vector3.one;

    private bool isDead = false;
    private AudioSource triggerAudioSource;
    private bool hasPlayedTriggerSound = false;

    [Header("Player Damage Settings")]
    public int damageAmount = 50;
    public float damageInterval = 1f;
    private float lastDamageTime;

    [Header("Boss Settings")]
    public bool isBoss = false; // Toggle for boss enemy

    private void Start()
    {
        currentHealth = maxHealth;
        triggerAudioSource = GetComponent<AudioSource>();
        lastDamageTime = Time.time;
    }

    private void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followDistance)
        {
            PlayTriggerSound();

            if (distance > stopDistance)
            {
                MoveTowardsPlayer();
            }

            RotateTowardsPlayer();
        }
        else
        {
            hasPlayedTriggerSound = false;
        }
    }

    private void PlayTriggerSound()
    {
        if (!hasPlayedTriggerSound && triggerSound != null && triggerAudioSource != null)
        {
            triggerAudioSource.PlayOneShot(triggerSound);
            hasPlayedTriggerSound = true;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (!Physics.Raycast(transform.position, direction, 0.75f))
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && Time.time >= lastDamageTime + damageInterval)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                lastDamageTime = Time.time;

                if (!isBoss) // Non-boss enemies die after dealing damage
                {
                    Die();
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy destroyed!");

        // Reward Player
        PlayerMoneyManager moneyManager = FindObjectOfType<PlayerMoneyManager>();
        if (moneyManager != null)
        {
            moneyManager.AddMoney(rewardAmount);
            Debug.Log($"Player received {rewardAmount} money! New balance: {moneyManager.GetTotalMoney()}");
        }
        else
        {
            Debug.LogWarning("PlayerMoneyManager script not found!");
        }

        // Play Death Sound
        if (deathAudioSource != null && deathSound != null)
        {
            deathAudioSource.PlayOneShot(deathSound, deathSoundVolume);
        }

        // Spawn Gibs
        if (gibPrefab != null)
        {
            GameObject spawnedGibs = Instantiate(gibPrefab, transform.position, Quaternion.identity);
            spawnedGibs.transform.localScale = gibSize;
            Destroy(spawnedGibs, 60f);
        }

        // Disable Enemy Components
        DisableEnemy();

        Destroy(gameObject, 0f);
    }

    private void DisableEnemy()
    {
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;
        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().isKinematic = true;
        if (GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;
        moveSpeed = 0;
    }
}
