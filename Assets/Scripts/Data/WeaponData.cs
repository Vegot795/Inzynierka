using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Inventory/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float range;
    public float fireRate;

    public Sprite weaponSprite;
    public GameObject projectile;
    public GameObject prefab;
    public Sprite muzzleFlash;
}
