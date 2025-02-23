using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SFPSC_FPSCamera : MonoBehaviour
{
    public static SFPSC_FPSCamera cam;
    private Camera cam_;

    public float sensitivity = 3;
    [HideInInspector]
    public float mouseX, mouseY;
    public float maxUpAngle = 80;
    public float maxDownAngle = -80;
    public Transform player;
    public Transform CameraPosition;

    private float rotX = 0.0f, rotY = 0.0f;
    [HideInInspector]
    public float rotZ = 0.0f;

    // Flag to control whether mouse look is enabled
    private bool isMouseLookEnabled = true;

    [Header("GameBoy Effect Settings")]
    [Tooltip("Place the palette Material you want (by default GameBoyShader)")]
    public Material palette;

    [Tooltip("The bigger downSampleSize --> the more pixelated (by default = 2)")]
    public int downsampleSize = 2;

    private void Awake()
    {
        cam = this;
        cam_ = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // Prevent camera movement if mouse look is disabled
        if (!isMouseLookEnabled)
            return;

        // Prevent camera movement if dialogue is active
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            return;
        }

        // Mouse input
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Calculate rotation
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, maxDownAngle, maxUpAngle);
        rotY += mouseX;

        // Apply calculated rotations
        transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
        player.Rotate(Vector3.up * mouseX);

        // Keep Camera position synced without breaking post-processing
        if (CameraPosition != null)
        {
            transform.position = CameraPosition.position;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (palette == null)
        {
            Debug.LogError("You must assign a palette Material to GameBoy Effect Script");
            Graphics.Blit(source, destination);
            return;
        }

        // Downsample
        int width = source.width / downsampleSize;
        int height = source.height / downsampleSize;

        RenderTexture temp = RenderTexture.GetTemporary(width, height, 0, source.format);
        temp.filterMode = FilterMode.Point; // Avoid interpolation for pixelated look

        // Obtain a smaller version of the source input
        Graphics.Blit(source, temp);

        // Apply GameBoy palette shader effect
        Graphics.Blit(temp, destination, palette);
        RenderTexture.ReleaseTemporary(temp);
    }

    // Method to disable or enable mouse look
    public void SetMouseLook(bool enable)
    {
        isMouseLookEnabled = enable;
    }

    public void Shake(float magnitude, float duration)
    {
        StartCoroutine(IShake(magnitude, duration));
    }

    private IEnumerator IShake(float mag, float dur)
    {
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        for (float t = 0.0f; t <= dur; t += Time.deltaTime)
        {
            rotZ = Random.Range(-mag, mag) * (1.0f - t / dur);
            yield return wfeof;
        }
        rotZ = 0.0f;
    }

    // Method to update sensitivity
    public void SetSensitivity(float value)
    {
        sensitivity = value;
    }
}
