using UnityEngine;

public class InteractableBase : PickupBase
{
    public MapGenerator.Corridor.Orientation orientation;
    public GameControllerScript GCS;
    public OverlayController OC;

    public override void Awake()
    {
        GCS = GameControllerScript.GCS;
        OC = OverlayController.OC;
    }

    public override void OnPickedUp(PlayerInventory inventory) { }
    public override void OnPlayerEnter() { }
    public override void OnPlayerExit() { }
}
