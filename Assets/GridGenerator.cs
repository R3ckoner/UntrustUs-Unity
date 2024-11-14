using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // For custom editor functionality
#endif

public class GridGenerator : MonoBehaviour
{
    public int gridWidth = 10;      // Number of columns
    public int gridHeight = 10;     // Number of rows
    public float cellSize = 1f;     // Size of each grid cell
    public GameObject gridCellPrefab; // Prefab for the grid cell
    public bool generateOnStart = false; // Allow grid to auto-generate at runtime
    public LayerMask plantableLayer; // Only allow planting on tiles
    public Material highlightMaterial; // Highlight material for valid tiles

    private Material originalMaterial;
    private GameObject highlightedTile;
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
        Debug.Log($"{seed.seedName} selected for planting.");
    }

    void Update()
    {
        HandleTileHighlighting();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedCrop == null)
            {
                Debug.LogWarning("No seed selected for planting!");
                return;
            }

            if (highlightedTile != null)
            {
                PlantSeed(highlightedTile.transform.position);
            }
            else
            {
                Debug.LogWarning("No tile to plant on!");
            }
        }
    }

    private void HandleTileHighlighting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, plantableLayer))
        {
            GameObject tile = hit.collider.gameObject;

            if (highlightedTile != tile)
            {
                ClearHighlight();
                HighlightTile(tile);
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void HighlightTile(GameObject tile)
    {
        highlightedTile = tile;
        Renderer tileRenderer = tile.GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalMaterial = tileRenderer.material;
            tileRenderer.material = highlightMaterial;
        }
        else
        {
            Debug.LogWarning($"Tile {tile.name} does not have a Renderer!");
        }
    }

    private void ClearHighlight()
    {
        if (highlightedTile != null)
        {
            Renderer tileRenderer = highlightedTile.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                tileRenderer.material = originalMaterial;
            }
            highlightedTile = null;
        }
    }

    private void PlantSeed(Vector3 position)
    {
        if (Physics.CheckSphere(position, 0.1f, plantableLayer))
        {
            Debug.LogWarning("Tile is already occupied!");
            return;
        }

        Instantiate(selectedCrop, position, Quaternion.identity);
        Debug.Log($"Planted {selectedCrop.name} at {position}");
        selectedCrop = null; // Clear after planting
        ClearHighlight();
    }
}
