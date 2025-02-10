using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the inventory UI GameObject
    public SFPSC_FPSCamera fpsCamera; // Reference to the SFPSC_FPSCamera script

    private bool isInventoryOpen = false;

    void Start()
    {
        inventoryUI.SetActive(false); // Ensure inventory starts hidden
        if (fpsCamera == null)
        {
            fpsCamera = FindObjectOfType<SFPSC_FPSCamera>(); // Auto-assign if missing
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Press "I" to toggle the inventory
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen); // Show/Hide inventory UI

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Unlock cursor
            Cursor.visible = true; // Show cursor
            fpsCamera.SetMouseLook(false); // Disable mouse look
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock cursor
            Cursor.visible = false; // Hide cursor
            fpsCamera.SetMouseLook(true); // Enable mouse look
        }
    }
}