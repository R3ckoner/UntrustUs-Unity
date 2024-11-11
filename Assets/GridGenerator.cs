using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int gridWidth = 10; // Number of columns
    public int gridHeight = 10; // Number of rows
    public float cellSize = 1f; // Size of each grid cell
    public GameObject gridCellPrefab; // Prefab for the grid cell
    public LayerMask plantableLayer; // Layer for plantable tiles

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject newCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);

                // Assign the plantable layer to each cell
                newCell.layer = LayerMask.NameToLayer("Plantable");

                // Optionally assign a tag to distinguish plantable cells
                newCell.tag = "Plantable";
            }
        }
    }
}

