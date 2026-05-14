using UnityEngine;

public class WeaponPickup : PickupBase
{
    public WeaponData weaponData;
    private SpriteRenderer sr;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (weaponData != null && sr != null)
        {
            sr.sprite = weaponData.weaponSprite;
        }
    }

    public override void OnPickedUp(PlayerInventory inventory)
    {
        inventory.PickupWeapon(this);
    }

    public static WeaponPickup SpawnFromData(WeaponData data, Vector2 position)
    {
        GameObject pickupObj = new GameObject($"Pickup_[{data.weaponName}]");
        pickupObj.transform.position = position;

        SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
        sr.sprite = data.weaponSprite;
        sr.sortingOrder = -1; 

        CircleCollider2D col = pickupObj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;

        WeaponPickup pickup = pickupObj.AddComponent<WeaponPickup>();
        pickup.weaponData = data;

        return pickup;
    }
}
