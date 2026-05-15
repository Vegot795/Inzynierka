using System.Linq;
using UnityEngine;

public enum WeaponType
{
    Rifle,
    Shotgun
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Inventory/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float range;
    public float fireRate;
    public WeaponType weaponType;

    public Sprite weaponSprite;
    public GameObject projectilePrefab;
    public GameObject prefab;
    public Sprite muzzleFlash;
}
