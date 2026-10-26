using UnityEngine;

public class BuyableBase : InteractableBase
{
    public GameObject Tip;
    public string objectName;
    public int price;

    public override void OnPlayerEnter()
    {
        OC.MakeBuyItemTip(this);
    }

    public override void OnPlayerExit()
    {
        if (Tip != null)
        {
            OC.DestroyBuyItemTip(this);
        }
    }
}
