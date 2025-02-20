using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    // Handle the drop event when an item is dragged over and dropped onto the slot
    public void OnDrop(PointerEventData eventData)
    {
        // Check if the slot is empty (no item currently present)
        if (transform.childCount == 0)
        {
            // Get the dropped object from the event data
            GameObject dropped = eventData.pointerDrag;
            
            // Check if the dropped object is a draggable item
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            
            if (draggableItem != null)
            {
                // Set the parent of the dragged item to this slot
                draggableItem.parentAfterDrag = transform;
                // Move the item to the slot
                dropped.transform.SetParent(transform);
                dropped.transform.localPosition = Vector3.zero; // Position item at the center of the slot
            }
        }
    }
}