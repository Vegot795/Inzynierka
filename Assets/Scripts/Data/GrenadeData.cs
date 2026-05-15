using UnityEngine;
[CreateAssetMenu(fileName = "GrenadeData", menuName = "Inventory/Grenade")]
public class GrenadeData : ScriptableObject
{
    public string grenadeName;
    public float explosionRadius = 3f;
    public float damage = 50f;
    public float moveSpeed = 15f;
    public Sprite grenadeSprite;
    public GameObject prefab;
    public Sprite grenadeOutline;
}
