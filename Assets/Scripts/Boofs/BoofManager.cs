using System.Collections.Generic;
using UnityEngine;

public class BoofManager : MonoBehaviour
{
    public static BoofManager boofManager { get; private set; }
    public UIController UIC;
    public GameControllerScript GCS;
    public PC_Controller player;
    public WeaponClass weapon;
    public Dictionary<BoofBoxData, GameObject> BoofBox = new Dictionary<BoofBoxData, GameObject>();
    
    public void Awake()
    {
        if (boofManager == null)
        {
            boofManager = this;
        }
        UIC = UIController.UIC;
        GCS = GameControllerScript.GCS;
        player = GCS.playerObj.GetComponent<PC_Controller>();
        
    }


    public void AddBoofToList(BoofBoxData boof)
    {
        BoofBoxData newBoofSo = Instantiate(boof);

        GameObject newBoofGO = Instantiate(boof.boofGO);
        newBoofGO.transform.SetParent(UIC.BoofsList.transform);
        BoofBox.Add(boof, boof.boofGO);
        boof.ActivateBoof(player); 
    }



}
