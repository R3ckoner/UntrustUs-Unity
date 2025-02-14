using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float followDistance = 5f;
    public float stopDistance = 0.025f;

    [Header("Audio Settings")]
    public AudioSource deathAudioSource; // Assign in Inspector for the death sound
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
    private AudioSource triggerAudioSource; // This will automatically reference the AudioSource attached to the enemy

    [Header("Player Damage Settings")]
    public int damageAmount = 50; // How much damage the enemy deals when it collides with the player
    public float damageInterval = 1f; // How frequently damage is dealt (in seconds)
    private float lastDamageTime;

    private void Start()
    {
        currentHealth = maxHealth;

        // Get the AudioSource attached to the enemy
        triggerAudioSource = GetComponent<AudioSource>();
        lastDamageTime = Time.time;
    }

private void Update()
{
    if (isDead) return;

    float distance = Vector3.Distance(transform.position, player.position);

    if (distance <= followDistance)
    {
        if (triggerSound != null && triggerAudioSource != null && !triggerAudioSource.isPlaying)
        {
            triggerAudioSource.PlayOneShot(triggerSound);
        }

        if (distance > stopDistance)
        {
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Raycast in the movement direction to detect obstacles
            if (!Physics.Raycast(transform.position, direction, 0.75f)) // Adjust ray distance as needed
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }

        // Rotate towards player
        Vector3 lookDirection = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}

    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        // Check if the player is in range and deal damage
        if (other.CompareTag("Player") && Time.time >= lastDamageTime + damageInterval)
        {
            // Call the player's TakeDamage method
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            {
                playerHealth.TakeDamage(damageAmount);
              //  lastDamageTime = Time.time; // Reset the damage interval timer
                Die();
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

        if (deathAudioSource != null && deathSound != null)
        {
            deathAudioSource.PlayOneShot(deathSound, deathSoundVolume);
        }

        if (gibPrefab != null)
        {
            GameObject spawnedGibs = Instantiate(gibPrefab, transform.position, Quaternion.identity);
            spawnedGibs.transform.localScale = gibSize;
            Destroy(spawnedGibs, 60f);
        }

        // Disable physics and collider before rotating
        GetComponent<Collider>().enabled = false;
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().isKinematic = true; // Prevent physics from interfering
        }

        GetComponent<MeshRenderer>().enabled = false;
        moveSpeed = 0;

        Destroy(gameObject, 0f);
    }
}
