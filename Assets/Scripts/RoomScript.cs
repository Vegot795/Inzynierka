using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string roomName;
    public RectInt bounds;
    public EnvironmentData environmentData;
    public List<GridCell> cells = new List<GridCell>();
    public List<Sprite> roomSprites;
    public List<Room> createdRooms = new List<Room>();

    public Room(string roomName, RectInt bounds, EnvironmentData environmentData)
    {
        this.roomName = roomName;
        this.bounds = bounds;
        this.environmentData = environmentData;

        SetSpritesFromEnvironment(environmentData);
    }

    public Room Create(List<GridCell> cells)
    {
        int newRoomCount = createdRooms.Count + 1;
        string newRoomName = "Room_" + newRoomCount;

        RectInt newBounds = GetRoomBounds(cells);

        Room newRoom = new Room(newRoomName, newBounds, null);
        newRoom.cells = cells;

        foreach (GridCell cell in cells)
        {
            cell.hasRoom = true;
            cell.roomOwner = newRoom;
        }

        createdRooms.Add(newRoom);

        Debug.Log("Created new room: " + newRoomName + " with bounds: " + newBounds);

        return newRoom;
    }

    public Room SeparateRoom(Room room)
    {
        return null;
    }

    public void SetRoomEnvironment(Room room, EnvironmentData environmentData)
    {
        if (room == null || environmentData == null)
        {
            return;
        }

        room.environmentData = environmentData;
        room.SetSpritesFromEnvironment(environmentData);

        foreach (GridCell cell in room.cells)
        {
            cell.environmentData = environmentData;

            if (cell.sr != null)
            {
                cell.sr.sprite = environmentData.Floor;
            }
        }
    }

    private void SetSpritesFromEnvironment(EnvironmentData environmentData)
    {
        if (environmentData == null)
        {
            return;
        }

        roomSprites = new List<Sprite>
        {
            environmentData.WallLeft,
            environmentData.WallRight,
            environmentData.WallTop,
            environmentData.WallBottom,
            environmentData.WallTopRight,
            environmentData.WallBottomRight,
            environmentData.WallTopLeft,
            environmentData.WallBottomLeft,
            environmentData.Floor
        };
    }

    private RectInt GetRoomBounds(List<GridCell> cells)
    {
        if (cells == null || cells.Count == 0)
        {
            return new RectInt(0, 0, 0, 0);
        }

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (GridCell cell in cells)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y < minY) minY = cell.y;
            if (cell.y > maxY) maxY = cell.y;
        }

        return new RectInt(
            minX,
            minY,
            maxX - minX + 1,
            maxY - minY + 1
        );
    }
}
