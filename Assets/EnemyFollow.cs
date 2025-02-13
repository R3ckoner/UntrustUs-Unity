using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float moveSpeed = 3f;  // Movement speed of the enemy
    public float followDistance = 5f;  // Distance at which the enemy starts following
    public float stopDistance = 0.025f;  // Distance (in meters) at which the enemy stops moving (about 1 inch)
    public AudioClip triggerSound;  // The sound to play when triggered
    private AudioSource audioSource;  // The AudioSource component

    [Header("Health Settings")]
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth; // Current health of the enemy

    private void Start()
    {
        // Get the AudioSource component attached to the enemy
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // Check distance between enemy and player
        float distance = Vector3.Distance(transform.position, player.position);

        // If player is within the follow distance
        if (distance <= followDistance)
        {
            // Play sound if it's not already playing
            if (!audioSource.isPlaying && triggerSound != null)
            {
                audioSource.PlayOneShot(triggerSound);
            }

            // If the enemy is further than the stop distance, continue moving toward the player
            if (distance > stopDistance)
            {
                // Make the enemy follow the player, but only in the X and Z axes
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            // Make the enemy face the player
            Vector3 direction = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // Method to deal damage to the player when the enemy collides with them
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(triggerSound);
            var playerHealth = other.GetComponent<PlayerHealth>(); // Assuming the player has a PlayerHealth script
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(50); // Deal 10 damage (or adjust as needed)
            }
            Destroy(gameObject); // Destroy the enemy
        }
    }

    // Method to take damage from the player's weapons
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
        Debug.Log("Enemy destroyed!");
        Destroy(gameObject); // Destroy the enemy
    }
}
