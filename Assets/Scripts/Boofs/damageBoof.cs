using UnityEngine;

public class damageBoof : BoofBoxData
{
    public float damageModif = 0.2f;

    public override void ActivateBoof(PC_Controller player)
    {
        var weapon = player.GetComponent<WeaponClass>();
        weapon.damageModif += damageModif;

    }

    public override void DeactivateBoof(PC_Controller player)
    {
        var weapon = player.GetComponent<WeaponClass>();
        weapon.damageModif -= damageModif;
    }
}
