using UnityEngine;

public class WeaponBuyScript : BuyableBase
{
    public WeaponData weaponData;
    public GameObject BoxCollider;

    public override void Awake()
    {
        base.Awake();
        objectName = weaponData.weaponName;
    }
    public override void OnPickedUp(PlayerInventory inventory)
    {
        if (inventory.currentScorePoints >= price)
        {
            inventory.currentScorePoints -= price;
            inventory.currentWeapon = weaponData;
            inventory.weaponHolder.EquipWeapon(inventory.currentWeapon);
            inventory.SetWeaponScriptReferences(inventory.currentWeapon);
        }
    }
}
