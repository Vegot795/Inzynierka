using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string roomName;
    public RectInt bounds;
    public EnvironmentData environmentData;
    public List<GridCell> cells = new List<GridCell>();
    public List<Room> createdRooms = new List<Room>();
    public int minimalRoomLength = 6;

    public Room(string roomName, List<GridCell> cells, EnvironmentData environmentData)
    {
        this.roomName = roomName;
        this.cells = cells;
        this.bounds = GetRoomBounds(cells);
        this.environmentData = environmentData;

        SetCellSpritesFromEnvironment(this, environmentData);
    }

    public Room CreateNewRoom(List<GridCell> cells)
    {
        int newRoomCount = createdRooms.Count + 1;
        string newRoomName = "Room_" + newRoomCount;

        Room newRoom = new Room(newRoomName, cells, null);

        foreach (GridCell cell in cells)
        {
            cell.hasRoom = true;
            cell.roomOwner = newRoom;
        }

        createdRooms.Add(newRoom);

        Debug.Log("Created new room: " + newRoomName + " with bounds: " + newRoom.bounds);

        return newRoom;
    }

    public (Room, Room) SeparateRoom(Room room)
    {
        if (room == null || room.cells == null || room.cells.Count <= 1)
        {
            Debug.LogWarning("[Room] Cannot separate null or too-small room.");
            return (null, null);
        }

        List<GridCell> roomCells = room.cells;
        int roomBoundsWidth = room.bounds.width;
        int roomBoundsHeight = room.bounds.height;
        if (roomBoundsWidth / 2 >= room.minimalRoomLength || roomBoundsHeight / 2 >= room.minimalRoomLength)
        {
            if (roomBoundsWidth >= roomBoundsHeight)
            {
                int midX = room.bounds.x + roomBoundsWidth / 2;
                List<GridCell> leftCells = new List<GridCell>();
                List<GridCell> rightCells = new List<GridCell>();

                foreach (GridCell cell in roomCells)
                {
                    if (cell.x < midX)
                    {
                        leftCells.Add(cell);
                    }
                    else
                    {
                        rightCells.Add(cell);
                    }
                }

                Room leftRoom = CreateNewRoom(leftCells);
                Room rightRoom = CreateNewRoom(rightCells);

                return (leftRoom, rightRoom);
            }
            else
            {
                int midY = room.bounds.y + roomBoundsHeight / 2;
                List<GridCell> topCells = new List<GridCell>();
                List<GridCell> bottomCells = new List<GridCell>();

                foreach (GridCell cell in roomCells)
                {
                    if (cell.y < midY)
                    {
                        bottomCells.Add(cell);
                    }
                    else
                    {
                        topCells.Add(cell);
                    }
                }

                Room topRoom = CreateNewRoom(topCells);
                Room bottomRoom = CreateNewRoom(bottomCells);

                return (topRoom, bottomRoom);
            }
        }
        else 
        { 
            Debug.LogWarning("[Room] Room is too small to separate.");
            return (null, null);
        }
    }

    public void SetRoomEnvironment(Room room, EnvironmentData environmentData)
    {
        if (room == null || environmentData == null)
        {
            return;
        }

        room.environmentData = environmentData;

        foreach (GridCell cell in room.cells)
        {
            cell.environmentData = environmentData;
        }

        SetCellSpritesFromEnvironment(room, environmentData);
    }

    private void SetCellSpritesFromEnvironment(Room room, EnvironmentData environmentData)
    {
        if (room == null || environmentData == null)
        {
            return;
        }

        foreach (GridCell cell in room.cells)
        {
            if (cell == null || cell.sr == null)
            {
                continue;
            }

            if (cell.type == GridCell.CellType.floor)
            {
                cell.sr.sprite = environmentData.Floor;
            }
            else if (cell.type == GridCell.CellType.wallTop)
            {
                cell.sr.sprite = environmentData.WallTop;
            }
            else if (cell.type == GridCell.CellType.wallBottom)
            {
                cell.sr.sprite = environmentData.WallBottom;
            }
            else if (cell.type == GridCell.CellType.wallLeft)
            {
                cell.sr.sprite = environmentData.WallLeft;
            }
            else if (cell.type == GridCell.CellType.wallRight)
            {
                cell.sr.sprite = environmentData.WallRight;
            }
            else if (cell.type == GridCell.CellType.cornerLeftTop)
            {
                cell.sr.sprite = environmentData.WallTopLeft;
            }
            else if (cell.type == GridCell.CellType.cornerRightTop)
            {
                cell.sr.sprite = environmentData.WallTopRight;
            }
            else if (cell.type == GridCell.CellType.cornerLeftBottom)
            {
                cell.sr.sprite = environmentData.WallBottomLeft;
            }
            else if (cell.type == GridCell.CellType.cornerRightBottom)
            {
                cell.sr.sprite = environmentData.WallBottomRight;
            }
        }
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
