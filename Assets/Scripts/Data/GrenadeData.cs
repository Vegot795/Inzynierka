using UnityEngine;

public enum GrenadeType
{
    Frag,
    Kasket,
    Flash
}

[CreateAssetMenu(fileName = "GrenadeData", menuName = "Inventory/Grenade")]
public class GrenadeData : ScriptableObject
{
    public string grenadeName;
    public float explosionRadius;
    public float damage;
    public float moveSpeed;

    public Sprite grenadeSprite;
    public GameObject prefab;
    public Sprite grenadeOutline;
    public GrenadeType grenadeType;
}
