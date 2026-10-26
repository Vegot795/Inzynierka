using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "dropItemsData", menuName = "Lists/DropItems")]
public class ItemDropData : ScriptableObject
{
    public List<GameObject> dropItems = new List<GameObject>();
}
