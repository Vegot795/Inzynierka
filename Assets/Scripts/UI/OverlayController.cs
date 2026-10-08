using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class OverlayController : MonoBehaviour
{
    public static OverlayController OC;
    public GameControllerScript GCS;
    public BoofManager BM;
    public GameObject tipPrefab;
    public Camera cam;
    public List<GameObject> tipList = new List<GameObject>();


    void Awake()
    {
        OC = this;
    }

    public void SetupOC()
    {
        GCS = GameControllerScript.GCS;
        BM = BoofManager.boofManager;
    }

    public Vector3 GetPositionForTip(GameObject tip)
    {
        TipController TC = tip.GetComponent<TipController>();
        Vector3 displayPosition = new Vector3();

        float xDistance = 1f;
        float yDistance = 1f;

        if (TC.owner is DoorScript)
        {
            xDistance = 1f;
            yDistance = 1f;
        }
        else if (TC.owner is BoofStationScript)
        {
            xDistance = 2f;
            yDistance = 1f;
        }

        if (TC.assignedObject.GetComponent<InteractableBase>().orientation == MapGenerator.Corridor.Orientation.Vertical)
        {
            if(GCS.playerObj.transform.position.y > TC.assignedObject.transform.position.y)
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x, TC.assignedObject.transform.position.y - yDistance, TC.assignedObject.transform.position.z);
            }
            else
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x, TC.assignedObject.transform.position.y + 1f, TC.assignedObject.transform.position.z);
            }
        }
        else if (TC.assignedObject.GetComponent<InteractableBase>().orientation == MapGenerator.Corridor.Orientation.Horizontal)
        {
            if (GCS.playerObj.transform.position.x > TC.assignedObject.transform.position.x)
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x - xDistance, TC.assignedObject.transform.position.y, TC.assignedObject.transform.position.z);
            }
            else
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x + xDistance, TC.assignedObject.transform.position.y, TC.assignedObject.transform.position.z);
            }
        }

        return displayPosition;
    }

    void Update()
    {
        foreach (var tip in tipList)
        {
            tip.transform.position = cam.WorldToScreenPoint(GetPositionForTip(tip));
        }
    }

    public void MakeDoorTip(DoorScript door)
    {
        if (GCS == null)
        {
            GCS = GameControllerScript.GCS;
        }
        GameObject tip = Instantiate(tipPrefab);
        tip.transform.SetParent(this.transform);
        TipController TC = tip.GetComponent<TipController>();
        TC.owner = door;
        TC.title.text = "Open";
        TC.cost.text = GCS.doorPrice.ToString();
        door.Tip = tip;
        TC.assignedObject = door.gameObject;
        tipList.Add(tip);
    }

    public void DestroyDoorTip(DoorScript door)
    {
        Debug.Log($"[OC] - {door.Tip.name} has to be deleted");
        tipList.Remove(door.Tip);
        Destroy(door.Tip);
    }

    public void MakeBoofStationTip(BoofStationScript BSS)
    {
        if (GCS == null)
        {
            GCS = GameControllerScript.GCS;
        }
        GameObject tip = Instantiate(tipPrefab);
        tip.transform.SetParent(this.transform);
        TipController TC = tip.GetComponent<TipController>();
        TC.owner = BSS;
        TC.title.text = BSS.BBD.boofName.ToString();
        
        if(!BM.BoofBox.ContainsKey(BSS.BBD))
        {
            TC.cost.text = BSS.BBD.boofCost.ToString();
        }
        else
        {
            TC.cost.text = "Already obtained";
        }

        BSS.Tip = tip;
        TC.assignedObject = BSS.gameObject;
        tipList.Add(tip);
        Debug.Log("Boof Tip Created");
    }

    public void DestroyBoofStationTip(BoofStationScript BSS)
    {
        Debug.Log($"[OC] - {BSS.Tip.name} has to be deleted");
        tipList.Remove(BSS.Tip);
        Destroy(BSS.Tip);
    }
}
