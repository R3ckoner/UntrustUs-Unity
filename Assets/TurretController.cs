using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    public Transform turretHead;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float rotationSpeed = 5f;
    public float fireRate = 1f;
    public float detectionRange = 20f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Audio Settings")]
    public AudioSource fireAudioSource;
    public AudioSource deathAudioSource;

    [Header("Rewards")]
    public int rewardAmount = 50; // Amount of money rewarded on turret kill

    private Transform player;
    private float fireTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        fireTimer = 0f;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            TrackPlayer();
            TryFire();
        }
    }

    void TrackPlayer()
    {
        Vector3 direction = player.position - turretHead.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Lerp(turretHead.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void TryFire()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= 1f / fireRate)
        {
            FireProjectile();
            fireTimer = 0f;
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            if (fireAudioSource != null)
            {
                fireAudioSource.Play();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Turret destroyed!");

        // Reward the player
        var playerMoneyManager = FindObjectOfType<PlayerMoneyManager>();
        if (playerMoneyManager != null)
        {
            playerMoneyManager.AddMoney(rewardAmount);
        }

        // Play death sound if available
        if (deathAudioSource != null)
        {
            deathAudioSource.Play();
        }

        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
