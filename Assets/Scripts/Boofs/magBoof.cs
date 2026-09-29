using UnityEngine;
using static Unity.Cinemachine.AxisState;

public class magBoof : BoofBoxData
{
    public float magModif = 0.2f;

    public override void ActivateBoof(PC_Controller player)
    {
        var weapon = player.GetComponent<WeaponClass>();
        weapon.maxAmmoModif += magModif;
        weapon.OnMagBoofStatusChange();
    }

    public override void DeactivateBoof(PC_Controller player)
    {
        var weapon = player.GetComponent<WeaponClass>();
        weapon.maxAmmoModif -= magModif;
        weapon.OnMagBoofStatusChange();
    }

}
