using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed = 10f; // Speed of automatic rotation

    void Update()
    {
        // Automatically rotate the globe around the Y-axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}