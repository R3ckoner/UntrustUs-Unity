using UnityEngine;

public class ToxicWater : MonoBehaviour
{
    [Header("Toxic Water Settings")]
    public Transform respawnPoint; // The point to move the player back to when interacting with the toxic water
    public KeyCode interactKey = KeyCode.E; // The key to interact with the toxic water (E key by default)

    private bool playerInRange = false;

    void Update()
    {
        // Check if the player is in range and presses the interact key
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            MovePlayerToRespawnPoint();
        }
    }

    private void MovePlayerToRespawnPoint()
    {
        if (respawnPoint != null)
        {
            // Move the player to the respawn point
            Transform player = GameObject.FindWithTag("Player").transform;
            player.position = respawnPoint.position;
            Debug.Log("Player moved back to the respawn point!");
        }
        else
        {
            Debug.LogWarning("Respawn point is not set in the inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player is near toxic water. Press interact to move.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left the toxic water interaction zone.");
        }
    }
}
