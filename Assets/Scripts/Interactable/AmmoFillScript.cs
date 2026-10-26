using UnityEngine;

public class AmmoFillScript : BuyableBase
{
    public override void Awake()
    {
        base.Awake();
        objectName = "AmmoFill";
    }
    public override void OnPickedUp(PlayerInventory inventory)
    {
        if (inventory.currentScorePoints >= price)
        {
            inventory.currentScorePoints -= price;
            inventory.weaponClass.currentReserveAmmo = inventory.weaponClass.currentMaxAmmo;
        }
    }


}
