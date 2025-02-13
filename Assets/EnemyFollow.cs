using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float moveSpeed = 3f;
    public float followDistance = 5f;
    public float stopDistance = 0.025f;
    public AudioClip triggerSound;
    private AudioSource audioSource;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Rewards")]
    public int rewardAmount = 50; // Amount of money rewarded on enemy kill

    [Header("Gib Settings")]
    public GameObject gibPrefab;

    private bool isDead = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followDistance)
        {
            if (!audioSource.isPlaying && triggerSound != null)
            {
                audioSource.PlayOneShot(triggerSound);
            }

            if (distance > stopDistance)
            {
                Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            Vector3 direction = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(triggerSound);
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(50);
            }
            Die();
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
        if (gibPrefab != null)
        {
            GameObject spawnedGibs = Instantiate(gibPrefab, transform.position, Quaternion.identity);
            Destroy(spawnedGibs, 30f);
        }
        
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy destroyed!");

        var playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.AddMoney(rewardAmount);
        }

      

        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        moveSpeed = 0;

        Destroy(gameObject, 0);
    }
}
