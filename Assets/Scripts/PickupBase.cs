using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{

    [Header("Pickup Settings")]
    public float pickupRadius = 0.6f;
    protected PickupOutline outline;

    protected virtual void Awake()
    {
        outline = GetComponent<PickupOutline>();
    }
    public abstract void OnPickedUp(PlayerInventory inventory);

    public virtual void OnPlayerEnter()
    {
       GetComponent<PickupOutline>()?.SetHovered(true);
    }
    public virtual void OnPlayerExit()
    {
        GetComponent<PickupOutline>()?.SetHovered(false);
    }
}
