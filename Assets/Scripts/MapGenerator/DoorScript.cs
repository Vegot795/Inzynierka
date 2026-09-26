using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public int price;
    public bool isLocked = true;
    public Room[] connectsRooms;
    public MapGenerator mapGenerator;
    public MapGenerator.Corridor belongedCorridor;
    public MapGenerator.Corridor.Orientation orientation;
    public SpriteRenderer sr;
    public Sprite VerticalSprite;
    public Sprite HorizontalSprite;
    public void Awake()
    {
        connectsRooms = new Room[2] { belongedCorridor.room1, belongedCorridor.room2 };
        orientation = belongedCorridor.orientation;

        if (orientation == MapGenerator.Corridor.Orientation.Vertical)
        {
            sr.sprite = VerticalSprite;
        }
        else
        {
            sr.sprite = HorizontalSprite;
        }

    }
}
