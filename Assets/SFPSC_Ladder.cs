using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFPSC_Ladder : MonoBehaviour
{
    private SFPSC_PlayerMovement playerMovement;
    private Rigidbody playerRb;
    private bool isClimbing = false;

    [Header("Ladder Settings")]
    public float climbSpeed = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<SFPSC_PlayerMovement>();
            playerRb = other.GetComponent<Rigidbody>();
            if (playerMovement != null && playerRb != null)
            {
                StartClimbing();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        playerMovement.DisableMovement(); // Disable normal movement
        playerRb.useGravity = false;
        playerRb.linearVelocity = Vector3.zero;
    }

    private void StopClimbing()
    {
        isClimbing = false;
        playerMovement.EnableMovement(); // Re-enable normal movement
        playerRb.useGravity = true;
    }

    private void Update()
    {
        if (isClimbing && playerMovement != null)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            playerRb.linearVelocity = new Vector3(0, verticalInput * climbSpeed, 0);
        }
    }
}
