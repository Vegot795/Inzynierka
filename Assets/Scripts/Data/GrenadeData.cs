using UnityEngine;
[CreateAssetMenu(fileName = "GrenadeData", menuName = "Inventory/Grenade")]
public class GrenadeData : ScriptableObject
{
    public string grenadeName;
    public string explosionRadius;
    public float damage;
    public Sprite grenadeSprite;
    public GameObject prefab;
}
