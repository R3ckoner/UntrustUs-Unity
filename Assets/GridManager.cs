using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject selectedCrop; // Crop prefab to plant (assigned via inventory selection)
    public LayerMask plantableLayer;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to plant
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, plantableLayer))
            {
                PlantSeed(hit.point);
            }
        }
    }

    void PlantSeed(Vector3 hitPoint)
    {
        if (selectedCrop == null)
        {
            Debug.Log("No seed selected!");
            return;
        }

        Vector3 gridPosition = new Vector3(
            Mathf.Round(hitPoint.x),
            hitPoint.y,
            Mathf.Round(hitPoint.z)
        );

        // Ensure we only plant on empty tiles
        if (!Physics.CheckSphere(gridPosition, 0.1f))
        {
            Instantiate(selectedCrop, gridPosition, Quaternion.identity);
            selectedCrop = null; // Clear after planting
        }
        else
        {
            Debug.Log("Tile is already occupied!");
        }
    }

    public void SelectSeed(SeedItem seed)
    {
        selectedCrop = seed.cropPrefab;
    }
}
