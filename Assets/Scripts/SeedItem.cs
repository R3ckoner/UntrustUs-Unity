using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "Inventory/SeedItem")]
public class SeedItem : ScriptableObject
{
    public string seedName;
    public GameObject cropPrefab; // Prefab of the crop to grow
    public Sprite seedIcon;       // (Optional) Icon for UI
}
