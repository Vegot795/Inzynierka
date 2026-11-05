using UnityEngine;

public class PickupOnCollision : MonoBehaviour
{
    public PlayerInventory inventory { get; protected set; }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerInventory>() != null)
        {
            inventory = collision.GetComponent<PlayerInventory>();
        }
        else
        {
            return;
        }
    }

}
