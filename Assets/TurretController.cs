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
    public int maxHealth = 100; // Turret's maximum health
    private int currentHealth;

    [Header("Audio Settings")]
    public AudioSource fireAudioSource;    
    public AudioSource deathAudioSource; // Optional death sound

    private Transform player;
    private float fireTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth; // Set initial health
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

        Debug.Log($"Turret hit! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Turret destroyed!");

        // Play death sound if available
        if (deathAudioSource != null)
        {
            deathAudioSource.Play();
        }

        // Optionally play a destruction animation here
        Destroy(gameObject, 0.5f); // Slight delay to allow death sound to play
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
