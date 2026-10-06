using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class OverlayController : MonoBehaviour
{
    public static OverlayController OC;
    public GameControllerScript GCS;
    public GameObject tipPrefab;
    public Camera cam;
    public List<GameObject> tipList = new List<GameObject>();


    void Awake()
    {
        OC = this;
        GCS = GameControllerScript.GCS;
    }

    public Vector3 GetPositionForTip(GameObject tip)
    {
        TipController TC = tip.GetComponent<TipController>();
        Vector3 displayPosition = new Vector3();

        if (TC.assignedObject.GetComponent<DoorScript>().orientation == MapGenerator.Corridor.Orientation.Vertical)
        {
            if(GCS.playerObj.transform.position.y > TC.assignedObject.transform.position.y)
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x, TC.assignedObject.transform.position.y - 1f, TC.assignedObject.transform.position.z);
            }
            else
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x, TC.assignedObject.transform.position.y + 1f, TC.assignedObject.transform.position.z);
            }
        }
        else if (TC.assignedObject.GetComponent<DoorScript>().orientation == MapGenerator.Corridor.Orientation.Horizontal)
        {
            if (GCS.playerObj.transform.position.x > TC.assignedObject.transform.position.x)
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x - 1f, TC.assignedObject.transform.position.y, TC.assignedObject.transform.position.z);
            }
            else
            {
                displayPosition = new Vector3(TC.assignedObject.transform.position.x + 1f, TC.assignedObject.transform.position.y, TC.assignedObject.transform.position.z);
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
        TC.title.text = BSS.BBD.boofName.ToString();
        TC.cost.text = BSS.BBD.boofCost.ToString();
        BSS.Tip = tip;
        TC.assignedObject = BSS.gameObject;
        tipList.Add(tip);
    }

    public void DestroyBoofStationTip(BoofStationScript BSS)
    {
        Debug.Log($"[OC] - {BSS.Tip.name} has to be deleted");
        tipList.Remove(BSS.Tip);
        Destroy(BSS.Tip);
    }
}
