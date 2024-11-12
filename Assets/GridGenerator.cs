using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // Required for custom editor functionality
#endif

public class GridGenerator : MonoBehaviour
{
    public int gridWidth = 10;      // Number of columns
    public int gridHeight = 10;     // Number of rows
    public float cellSize = 1f;     // Size of each grid cell
    public GameObject gridCellPrefab; // Prefab for the grid cell
    public bool generateOnStart = false; // Allow grid to auto-generate at runtime
    public LayerMask plantableLayer; // Only allow planting on tiles

    private GameObject selectedCrop; // Crop prefab from selected seed

    public void GenerateGrid()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("Grid Cell Prefab is not assigned.");
            return;
        }

        // Clear existing grid
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        Vector3 startPosition = transform.position;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 cellPosition = startPosition + new Vector3(x * cellSize, 0, z * cellSize);
                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                newCell.name = $"GridCell_{x}_{z}";
                newCell.layer = LayerMask.NameToLayer("Plantable");
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Undo.RegisterCreatedObjectUndo(newCell, "Create Grid Cell");
                }
#endif
            }
        }
    }

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateGrid();
        }
    }

    public void SelectSeed(SeedItem seed)
    {
        selectedCrop = seed.cropPrefab;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && selectedCrop != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, plantableLayer))
            {
                PlantSeed(hit.point);
            }
        }
    }

    private void PlantSeed(Vector3 hitPoint)
    {
        Vector3 gridPosition = new Vector3(
            Mathf.Round(hitPoint.x),
            0, // Keep crops aligned with grid
            Mathf.Round(hitPoint.z)
        );

        if (!Physics.CheckSphere(gridPosition, 0.1f, plantableLayer))
        {
            Instantiate(selectedCrop, gridPosition, Quaternion.identity);
            selectedCrop = null; // Clear seed after planting
        }
        else
        {
            Debug.Log("Tile is already occupied!");
        }
    }
}


