using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoof ", menuName = "Env/SpeedBoof")]

public class speedBoof : BoofBoxData
{
    public float speedModif = 0.2f;

    public override void ActivateBoof(PC_Controller player)
    {
        player.moveSpeedModif += speedModif;
    }

    public override void DeactivateBoof(PC_Controller player)
    {
        player.moveSpeedModif -= speedModif;
    }
}
