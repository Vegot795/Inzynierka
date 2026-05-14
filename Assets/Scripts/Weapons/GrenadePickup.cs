using UnityEngine;

public class GrenadePickup : PickupBase
{
    public GrenadeData grenadeData;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (grenadeData != null && sr != null)
        {
            sr.sprite = grenadeData.grenadeSprite;
        }
    }

    public override void OnPickedUp(PlayerInventory inventory)
    {
        inventory.PickupGrenade(this);
    }

    public static GrenadePickup SpawnFromData(GrenadeData data, Vector2 position)
    {
        GameObject pickupObj = new GameObject($"Pickup_[{data.grenadeName}]");
        pickupObj.transform.position = position;

        SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
        sr.sprite = data.grenadeSprite;
        sr.sortingOrder = -1; 

        CircleCollider2D col = pickupObj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;

        GrenadePickup pickup = pickupObj.AddComponent<GrenadePickup>();
        pickup.grenadeData = data;

        return pickup;
    }
}
