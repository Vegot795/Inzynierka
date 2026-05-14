using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Current Slots (runtime, readonly)")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GrenadeData currentGrenade;

    [Header("References")]
    public WeaponHolder weaponHolder;

    [Header("Drop Settings")]
    public float dropDistanve = 0.8f;

    public WeaponData CurrentWeapon => currentWeapon;
    public GrenadeData CurrentGrenade => currentGrenade;
    public WeaponData startingWeapon;
    public GrenadeData stastingGrenade;

    private void Start()
    {
        if (startingWeapon != null)
        {
            currentWeapon = startingWeapon;
            weaponHolder.EquipWeapon(currentWeapon);
        }

        if(stastingGrenade != null)
        {
            currentGrenade = stastingGrenade;
        }
    }

    public void PickupWeapon(WeaponPickup pickup)
    {
        if (currentWeapon != null)
        {
            DropWeapon();
        }
        
        currentWeapon = pickup.weaponData;
        weaponHolder.EquipWeapon(currentWeapon);
        Destroy(pickup.gameObject);

        Debug.Log($"[Inventory] Equipped weapon: {currentWeapon.weaponName}");
    }

    public void PickupGrenade(GrenadePickup pickup)
    {
        if (currentGrenade != null)
        {
            DropGrenade();
        }
        currentGrenade = pickup.grenadeData;
        Destroy(pickup.gameObject);

        Debug.Log($"[Inventory] Picked up grenade: {currentGrenade.grenadeName}");
    }

    private void DropWeapon()
    {
        Vector2 dropPos = (Vector2)transform.position + GetDropOffset();
        WeaponPickup.SpawnFromData(currentWeapon, dropPos);
        currentWeapon = null;
        weaponHolder.UnequipWeapon();
    }

    private void DropGrenade()
    {
        Vector2 dropPos = (Vector2)transform.position + GetDropOffset();
        GrenadePickup.SpawnFromData(currentGrenade, dropPos);
        currentGrenade = null;
    }

    private Vector2 GetDropOffset()
    {
        return Vector2.left * dropDistanve;
    }

    private void OnInteract()
    {

    }
}
