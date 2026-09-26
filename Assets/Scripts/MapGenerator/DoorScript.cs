using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static MapGenerator;

public class DoorScript : PickupBase
{
    public int price;
    public bool isLocked = true;
    private bool openingEnded = false;
    public Room[] connectsRooms;
    public MapGenerator mapGenerator;
    public MapGenerator.Corridor belongedCorridor;
    public MapGenerator.Corridor.Orientation orientation;
    public Collider2D col;
    public GameObject RightWing;
    public GameObject LeftWing;
    private Rigidbody2D rrb;
    private Rigidbody2D lrb;
    [SerializeField] private Vector3 moveVector = new Vector3(1.5f , 0, 0);
    public float moveSpeed = 2f;
    public override void Awake()
    {
        
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
        Debug.Log($"[DoorScript] - beggins to open {this.name} the door");
        inventory.RemoveScore(price);

        rrb.MovePosition(Vector3.MoveTowards(transform.position, transform.position + moveVector, moveSpeed * Time.deltaTime));
        lrb.MovePosition(Vector3.MoveTowards(transform.position, transform.position - moveVector, moveSpeed * Time.deltaTime));

        if(rrb.transform.position == new Vector3(gameObject.transform.position.x - 1, 0,0) &&
           lrb.transform.position == new Vector3(gameObject.transform.position.x - 1, 0,0))
        {
            openingEnded = true;
            Debug.Log($"Doors {this.name} has been opened");
        }
    }
}
