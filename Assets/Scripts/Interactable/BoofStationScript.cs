using NUnit.Framework;
using System;
using UnityEngine;

public class BoofStationScript : InteractableBase
{
    public BoofManager boofManager;
    public BoofBoxData BBD;
    public SpriteRenderer sr;
    public GameObject Tip;
    public GameObject boxCollider;

    public override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        boofManager = BoofManager.boofManager;
    }

    public void SetUpBoofStation()
    {
        sr.sprite = BBD.boofStationSprite;
        /*BoxCollider2D bc = gameObject.AddComponent<BoxCollider2D>();
        bc.transform.parent = boxCollider.transform;
        bc.offset = new Vector2(0, 0.37f);
        bc.size = new Vector2(1.5f, 0.76f);

        CircleCollider2D cc = gameObject.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
        cc.radius = 1.5f;*/
    }

    public override void OnPlayerEnter()
    {
        OC.MakeBoofStationTip(this);
    }

    public override void OnPlayerExit()
    {
        if (Tip != null)
        {
            OC.DestroyBoofStationTip(this);
        }
    }

    public override void OnPickedUp(PlayerInventory inventory)
    {
        if(!boofManager.BoofBox.ContainsKey(BBD))
        {
            if (inventory.currentScorePoints >= BBD.boofCost)
            {
                inventory.RemoveScore(BBD.boofCost);
                boofManager.AddBoofToList(BBD);
                if (Tip != null)
                {
                    OC.DestroyBoofStationTip(this);
                }
            }
        }
    }
}
