using UnityEngine;

public class HealthDropScript : PickupOnCollision
{
    public int extraHP = 20;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (base.inventory != null)
        {
            var inventory = base.inventory;
        }
        else
        {
            return;
        }
        var player = inventory.GetComponent<PC_Controller>();
        if (player.CurrentHp >= player.currentMaxHp)
        {
            return;
        }
        float playerCurrentHP = player.CurrentHp;
        float playerCurrentMaxHP = player.currentMaxHp;

        float hpToAdd = Mathf.Min(extraHP, playerCurrentMaxHP - playerCurrentHP);
        player.CurrentHp += hpToAdd;
        Destroy(gameObject);
    }
}
