using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static MapGenerator;

public class DoorScript : PickupBase
{
    public int price;
    public bool isUnlocked = false;
    public bool openingEnded = false;
    public Room[] connectsRooms;
    public GameControllerScript GCS;
    public MapGenerator mapGenerator;
    public MapGenerator.Corridor belongedCorridor;
    public List<Room> RoomsAttached = new List<Room>();
    public MapGenerator.Corridor.Orientation orientation;
    public Collider2D col;
    public GameObject RightWing;
    public GameObject LeftWing;
    private Rigidbody2D rrb;
    private Rigidbody2D lrb;
    [SerializeField] private Vector3 moveVector = new Vector3(1.4f , 0, 0);
    private Vector3 rrbTarget;
    private Vector3 lrbTarget;
    public float moveSpeed = 0.2f;
    public override void Awake()
    {
        GCS = GameControllerScript.GCS;
    }

    private void Update()
    {
        if (isUnlocked && !openingEnded && RoomsAttached.Any(x => x.isUnlocked))
        {
            RightWing.transform.localPosition = Vector2.MoveTowards(RightWing.transform.localPosition, rrbTarget, moveSpeed * Time.deltaTime);
            LeftWing.transform.localPosition = Vector2.MoveTowards(LeftWing.transform.localPosition, lrbTarget, moveSpeed * Time.deltaTime);

            if (RightWing.transform.localPosition == rrbTarget &&
               LeftWing.transform.localPosition == lrbTarget)
            {
                openingEnded = true;
                //Debug.Log($"Doors {this.name} has been opened");
            }
        }
    }

    public void DoorSetup()
    {
        connectsRooms = new Room[2] { belongedCorridor.room1, belongedCorridor.room2 };
        //orientation = belongedCorridor.orientation;

        if (orientation == MapGenerator.Corridor.Orientation.Horizontal)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
            gameObject.transform.position = new Vector2(belongedCorridor.leftTopCell.transform.position.x + 0.1f, belongedCorridor.leftTopCell.transform.position.y - 0.5f);

        }
        else
        {
            gameObject.transform.position = new Vector2(belongedCorridor.leftTopCell.transform.position.x + 0.5f, belongedCorridor.leftTopCell.transform.position.y - 0.1f);
        }
        rrb = RightWing.GetComponent<Rigidbody2D>();
        lrb = LeftWing.GetComponent<Rigidbody2D>();
    }

    public override void OnPickedUp(PlayerInventory inventory)
    {
        //Debug.Log($"[DoorScript] - beggins to open {this.name} the door");
        inventory.RemoveScore(GCS.doorPrice);
        UnlockDoor();
        
    }

    public void UnlockDoor()
    {

        if (!isUnlocked) 
        {
            isUnlocked = true;

            rrbTarget = RightWing.transform.localPosition + moveVector;
            lrbTarget = LeftWing.transform.localPosition - moveVector;

            Debug.Log($"RightWing position: {RightWing.transform.position}, localPosition: {RightWing.transform.localPosition} Position to go: {rrbTarget}");
            Debug.Log($"RightWing position: {RightWing.transform.position}, localPosition: {RightWing.transform.localPosition}Position to go: {lrbTarget}");

            Room roomToOpen = RoomsAttached
                .Where(x => x.isUnlocked == false)
                .First();
            roomToOpen.UnlockRoom();
            
        } 
    }
}
