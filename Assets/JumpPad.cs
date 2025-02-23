using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 10f; // The force to apply when the player hits the jump pad

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided with the jump pad has a Rigidbody
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply an upward force to the player's Rigidbody
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
