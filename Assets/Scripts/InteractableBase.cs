using UnityEngine;

public class InteractableBase : PickupBase
{
    public MapGenerator.Corridor.Orientation orientation;
    public GameControllerScript GCS;
    public OverlayController OC;
    public MapGenerator MG;
    public MapGenerator.Directions direction;


    public override void Awake()
    {
        GCS = GameControllerScript.GCS;
        OC = OverlayController.OC;
        MG = MapGenerator.MG;
    }
    public virtual void OnInteraction() { }
    public override void OnPickedUp(PlayerInventory inventory) => OnInteraction();
    public override void OnPlayerEnter() { }
    public override void OnPlayerExit() { }

    public int GetRotation()
    {
        int cusRotation = 0;
        switch (direction)
        {
            case MapGenerator.Directions.Up:
                cusRotation = 0;
                break;
            case MapGenerator.Directions.Left:
                cusRotation = 90;
                break;
            case MapGenerator.Directions.Down:
                cusRotation = 180;
                break;
            case MapGenerator.Directions.Right:
                cusRotation = 270;
                break;
            default:
                Debug.LogWarning("[SpawnerScript] - Quaternion is screwwwwed");
                return 0;
        }

        return cusRotation;
    }
}
