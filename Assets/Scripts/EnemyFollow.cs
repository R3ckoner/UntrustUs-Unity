using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float followDistance = 5f;
    public float stopDistance = 0.025f;

    [Header("Audio Settings")]
    public AudioSource deathAudioSource;
    public AudioClip triggerSound;
    public AudioClip deathSound;
    [Range(0f, 1f)] public float deathSoundVolume = 1f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Rewards")]
    public int rewardAmount = 50;

    [Header("Gib Settings")]
    public GameObject gibPrefab;
    public Vector3 gibSize = new Vector3(1f, 1f, 1f);

    private bool isDead = false;
    private AudioSource triggerAudioSource;
    private float lastTriggerSoundTime = 0f;
    public float triggerSoundCooldown = 10f;
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
            if (!hasPlayedTriggerSound && triggerSound != null && triggerAudioSource != null)
            {
                triggerAudioSource.PlayOneShot(triggerSound);
                hasPlayedTriggerSound = true;
            }

            if (distance > stopDistance)
            {
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                Vector3 direction = (targetPosition - transform.position).normalized;

                if (!Physics.Raycast(transform.position, direction, 0.75f))
                {
                    transform.position += direction * moveSpeed * Time.deltaTime;
                }
            }

            Vector3 lookDirection = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        else
        {
            hasPlayedTriggerSound = false;
        }
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
                
                if (!isBoss) // Only die on collision if NOT a boss
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

    // Grant reward to the player
    PlayerMoneyManager moneyManager = FindObjectOfType<PlayerMoneyManager>();
    if (moneyManager != null)
    {
        moneyManager.AddMoney(rewardAmount);
        Debug.Log($"Player received {rewardAmount} money! New balance: {PlayerMoneyManager.savedTotalMoney}");
    }
    else
    {
        Debug.LogWarning("PlayerMoneyManager script not found!");
    }

    if (deathAudioSource != null && deathSound != null)
    {
        deathAudioSource.PlayOneShot(deathSound, deathSoundVolume);
    }

    if (gibPrefab != null)
    {
        // Instantiate multiple gibs and randomize their rotations
        for (int i = 0; i < 5; i++) // You can adjust the number of gibs here
        {
            // Create the gib object at the enemy's position
            GameObject spawnedGibs = Instantiate(gibPrefab, transform.position, Quaternion.identity);
            spawnedGibs.transform.localScale = gibSize;

            // Randomize rotation: Use random rotation around the enemy's up vector (assuming up is the "forward" direction)
            float randomX = Random.Range(-180f, 180f);
            float randomY = Random.Range(-180f, 180f);
            float randomZ = Random.Range(-180f, 180f);
            spawnedGibs.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);

            // Apply some force for a more natural explosion effect (optional)
            Rigidbody gibRb = spawnedGibs.GetComponent<Rigidbody>();
            if (gibRb != null)
            {
                gibRb.AddExplosionForce(10f, transform.position, 5f); // You can adjust the force and radius
            }

            // Destroy gib after a while
            Destroy(spawnedGibs, 10f);
        }
    }

    GetComponent<Collider>().enabled = false;
    if (GetComponent<Rigidbody>() != null)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    GetComponent<MeshRenderer>().enabled = false;
    moveSpeed = 0;

    Destroy(gameObject, 0f);
}


}
