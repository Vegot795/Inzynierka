using UnityEngine;

public class WeaponBuyScript : BuyableBase
{
    public WeaponData weaponData;
    public WeaponClass weaponClass;
    public GameObject BoxCollider;

    public override void Awake()
    {
        base.Awake();
        objectName = weaponData.weaponName;
    }
    public override void OnPickedUp(PlayerInventory inventory)
    {
        if (inventory.currentScorePoints >= price && inventory.weaponClass.weaponData != weaponData)
        {
            inventory.currentScorePoints -= price;
            inventory.currentWeapon = weaponData;
            inventory.weaponHolder.EquipWeapon(inventory.currentWeapon);
            inventory.SetWeaponScriptReferences(inventory.currentWeapon);
            inventory.PlaySFX(inventory.buyItemSFX);
            OC.DestroyBuyItemTip(this);
        }
    }
}
