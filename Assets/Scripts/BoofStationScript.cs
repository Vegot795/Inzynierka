using NUnit.Framework;
using System;
using UnityEngine;

public class BoofStationScript : InteractableBase
{
    public GameControllerScript GCS;
    public OverlayController OC;
    public BoofManager boofManager;
    public BoofBoxData BBD;
    public SpriteRenderer sr;
    public GameObject Tip;

    public override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = BBD.boofStationSprite;
        boofManager = BoofManager.boofManager;
    }

    public override void OnPlayerEnter()
    {
        if (!boofManager.BoofBox.ContainsKey(BBD))
        {
            OC.MakeBoofStationTip(this);
        }
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
            }
        }
    }
}
