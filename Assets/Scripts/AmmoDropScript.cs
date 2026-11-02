using UnityEngine;

public class AmmoDropScript : PickupBase
{
    public int extraMags = 1;

    public override void OnPickedUp(PlayerInventory inventory)
    {
        if (inventory.weaponClass == null || inventory.weaponClass.currentReserveAmmo == inventory.weaponClass.currentMaxAmmo)
        {
            return;
        }
        var curWeapon = inventory.weaponClass;
        if (curWeapon.currentReserveAmmo < curWeapon.currentMaxAmmo)
        {
            curWeapon.currentReserveAmmo = Mathf.Min(curWeapon.currentReserveAmmo + (curWeapon.maxMagAmmo * extraMags), curWeapon.currentMaxAmmo);

            /*var missingAmmo = curWeapon.currentMaxAmmo - curWeapon.currentReserveAmmo;
            if (missingAmmo >= extraMags * curWeapon.maxMagAmmo)
            {
                curWeapon.currentReserveAmmo += extraMags * curWeapon.maxMagAmmo;
            }
            else
            {
                curWeapon.currentReserveAmmo += missingAmmo;
            }*/
        }
        inventory.PlaySFX(inventory.pickupItemSFX);
        Destroy(gameObject);
    }
}
