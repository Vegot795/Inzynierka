using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Current Slots (runtime, readonly)")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GrenadeData currentGrenade;

    [Header("References")]
    public WeaponHolder weaponHolder;

    [Header("Drop Settings")]
    public float dropDistance = 0.8f;

    public WeaponData CurrentWeapon => currentWeapon;
    public GrenadeData CurrentGrenade => currentGrenade;
    public WeaponData startingWeapon;
    public GrenadeData stastingGrenade;

    public RiffleScript RS;
    private ShotgunScript SS;
    public PC_Controller PC;

    private void Start()
    {
        PC = GetComponent<PC_Controller>();

        if (startingWeapon != null)
        {
            currentWeapon = startingWeapon;
            weaponHolder.EquipWeapon(currentWeapon);
            SetWeaponScriptReferences(currentWeapon);
        }

        if (stastingGrenade != null)
        {
            currentGrenade = stastingGrenade;
        }
    }
    #region --- Pickup/Drop ---
    public void PickupWeapon(WeaponPickup pickup)
    {
        if (currentWeapon != null)
        {
            DropWeapon();
        }

        currentWeapon = pickup.weaponData;
        weaponHolder.EquipWeapon(currentWeapon);
        SetWeaponScriptReferences(currentWeapon);
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

    #endregion
    private Vector2 GetDropOffset()
    {
        return Vector2.left * dropDistance;
    }

    private void OnInteract()
    {

    }

    public void OnAttack()
    {
        if (currentWeapon != null)
        {
            if (currentWeapon.weaponType == WeaponType.Rifle)
            {
                RS.Shoot(PC.mousePos);
            }
            if (currentWeapon.weaponType == WeaponType.Shotgun)
            {
                SS.Shoot(PC.mousePos);
            }
        }
    }

    private void OnThrowGrenade()
    {

    }

    private void SetWeaponScriptReferences(WeaponData weapon)
    {
        RS = null;
        SS = null;

        // Get the script from the INSTANTIATED weapon, not the prefab
        GameObject weaponInstance = weaponHolder.CurrentWeaponInstance;
        if (weaponInstance == null) return;

        if (weapon.weaponType == WeaponType.Rifle)
        {
            RS = weaponInstance.GetComponent<RiffleScript>();
        }
        else if (weapon.weaponType == WeaponType.Shotgun)
        {
            SS = weaponInstance.GetComponent<ShotgunScript>();
        }
    }
}
