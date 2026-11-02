using UnityEngine;

[CreateAssetMenu(fileName = "HealthBoof ", menuName = "Env/HealthBoof")]

public class healthBoof : BoofBoxData
{
    public float healthModifier = 0.5f;

    public override void ActivateBoof(PC_Controller player)
    {
        player.healthModif += healthModifier;
        player.CurrentHp += player.BaseMaxHp * healthModifier;
        player.RecalculateHealth();
    }

    public override void DeactivateBoof(PC_Controller player)
    {
        player.healthModif -= healthModifier;
        player.CurrentHp -= player.BaseMaxHp * healthModifier;
        player.RecalculateHealth();
    }
}
