using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoofManager : MonoBehaviour
{
    public static BoofManager boofManager { get; private set; }
    public UIController UIC;
    public GameControllerScript GCS;
    public PC_Controller player;
    public WeaponClass weapon;
    public Dictionary<BoofBoxData, GameObject> BoofBox = new Dictionary<BoofBoxData, GameObject>();
    
    public void SetupBM()
    {
        if (boofManager == null)
        {
            boofManager = this;
        }
        UIC = UIController.UIC;
        GCS = GameControllerScript.GCS;
        player = GCS.playerObj.GetComponent<PC_Controller>();
        weapon = GCS.playerObj.GetComponent<PlayerInventory>().weaponClass;
    }


    public void AddBoofToList(BoofBoxData boof)
    {
        BoofBoxData newBoofSo = Instantiate(boof);

        GameObject newBoofGO = Instantiate(boof.boofGO);
        newBoofGO.GetComponent<Image>().sprite = boof.boofIcon;
        newBoofGO.transform.SetParent(UIC.BoofList.transform);
        BoofBox.Add(boof, boof.boofGO);
        boof.ActivateBoof(player); 
    }



}
