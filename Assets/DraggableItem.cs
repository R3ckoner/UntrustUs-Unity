using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // The Image component that will be dragged
    public Image image;
    
    // To store the original parent of the item
    [HideInInspector] public Transform parentAfterDrag;

    // Public variable for how many inventory slots this item takes up
    public Vector2Int slotsRequired = new Vector2Int(1, 1); // Default to 1x1 (1 slot)

    // Called when the drag starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the original parent
        parentAfterDrag = transform.parent;
        
        // Temporarily detach from the parent and set it to the root
        transform.SetParent(transform.root);
        
        // Bring it to the top of the hierarchy so it stays in front
        transform.SetAsLastSibling();
        
        // Disable raycasting so it doesn’t block UI interactions while dragging
        image.raycastTarget = false;
    }

    // Called during the dragging process
    public void OnDrag(PointerEventData eventData)
    {
        // Update the position to follow the mouse
        transform.position = Input.mousePosition;
    }

    // Called when the drag ends
    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the parent back to the original
        transform.SetParent(parentAfterDrag);
        
        // Re-enable raycasting for UI interaction
        image.raycastTarget = true;
    }

    // Call this to check how many slots the item will occupy
    public bool CanFitInSlots(Vector2Int gridSize)
    {
        return gridSize.x >= slotsRequired.x && gridSize.y >= slotsRequired.y;
    }
}