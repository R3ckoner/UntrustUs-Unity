using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform playerCamera;
    public float fixedXRotation = 90f;

    void Update()
    {
        if (playerCamera != null)
        {
            Vector3 direction = playerCamera.position - transform.position;
            direction.y = 0; // Keep the billboard upright
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(fixedXRotation, rotation.eulerAngles.y, 0);
            }
        }
    }
}