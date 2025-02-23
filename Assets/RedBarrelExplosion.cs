using UnityEngine;

public class RedBarrelExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Explosion effect to spawn
    public float damageThreshold = 10f; // Minimum damage needed to explode
    public float explosionRadius = 5f; // Radius in which enemies will take damage
    public float explosionForce = 500f; // Force applied to nearby rigidbodies

    public int damageAmount = 100;

    private bool isExploded = false;
    private MeshRenderer meshRenderer; // To hide the barrel mesh

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>(); // Cache the MeshRenderer
    }

    // This method will be called from the RaycastWeapon when it hits the barrel
    public void ApplyDamage(float damage)
    {
        if (isExploded) return;

        // Check if the damage exceeds the threshold
        if (damage >= damageThreshold)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (isExploded) return;

        isExploded = true;

        // Hide the barrel (make it invisible)
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Disable the MeshRenderer
        }

        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // Destroy the barrel after 1 second to allow explosion effect to play
        Destroy(gameObject, 1f);

        // Apply explosion force to nearby objects (e.g., rigidbodies)
        ApplyExplosionForce();

        // Damage enemies within the explosion radius
        DamageNearbyEnemies();
    }

    private void ApplyExplosionForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius); // Find nearby objects within radius

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply force to simulate explosion knockback
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    private void DamageNearbyEnemies()
    {
        // Get all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Check if the object is a player and apply damage
            if (nearbyObject.CompareTag("Player"))
            {
                PlayerHealth playerHealth = nearbyObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                }
            }

            // Check if the object is an enemy and apply damage
            if (nearbyObject.CompareTag("Enemy"))
            {
                EnemyFollow enemyHealth = nearbyObject.GetComponent<EnemyFollow>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
            }
        }
    }

}
