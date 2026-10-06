using UnityEngine;

[CreateAssetMenu(fileName = "BoofData ", menuName = "Env/BoofBox")]
public class BoofBoxData : ScriptableObject
{
    public string boofName;
    public int boofCost;
    public GameObject boofGO;
    public BoofTypes boofType;
    public Sprite boofStationSprite;


    public enum BoofTypes
    {
        speedBoof,
        healthBoof,
        afterlifeBoof,
        magBoof,
        armorBoof,
        damageBoof
    }

    public virtual void ActivateBoof(PC_Controller player) { }

    public virtual void DeactivateBoof(PC_Controller player) { }
}
