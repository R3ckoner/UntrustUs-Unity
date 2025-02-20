#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default fields

        GridGenerator gridGenerator = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridGenerator.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            for (int i = gridGenerator.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridGenerator.transform.GetChild(i).gameObject);
            }
        }
    }
}
#endif
