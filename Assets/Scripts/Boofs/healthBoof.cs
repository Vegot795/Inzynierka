using UnityEngine;
using static Unity.Cinemachine.AxisState;
[CreateAssetMenu(fileName = "HealthBoof ", menuName = "Env/HealthBoof")]

public class healthBoof : BoofBoxData
{
    public float healthModifier = 0.2f;

    public override void ActivateBoof(PC_Controller player)
    {
        player.healthModif += healthModifier;
        player.CurrentHp += player.BaseMaxHp * healthModifier;

    }

    public override void DeactivateBoof(PC_Controller player)
    {
        player.healthModif -= healthModifier;
        player.CurrentHp -= player.BaseMaxHp * healthModifier;

    }
}
