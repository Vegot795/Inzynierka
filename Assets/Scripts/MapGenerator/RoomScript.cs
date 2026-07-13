using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room
{
    public string roomName;
    public int minimalRoomLength = 6;
    public int roomsConnectedWithCorridor = 0;
    public RectInt bounds;
    public EnvironmentData environmentData;
    public List<GridCell> cells = new List<GridCell>();
    public List<Room> createdRooms = new List<Room>();
    public MapGenerator mapGenerator;

    public Room(string roomName, List<GridCell> cells, EnvironmentData environmentData)
    {
        this.roomName = roomName;
        this.cells = cells;
        this.bounds = GetRoomBounds(cells);
        this.environmentData = environmentData;

        SetCellSpritesFromEnvironment(this, environmentData);
    }

    public enum RoomsAdjected
    {
        Left,
        Right,
        Top,
        Bottom

    }

    public Dictionary<RoomsAdjected, Room> adjectedRooms = new Dictionary<RoomsAdjected, Room>();

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

            switch (cell.type)
            {
                case GridCell.CellType.floor:
                    cell.sr.sprite = environmentData.Floor;
                    break;

                case GridCell.CellType.wallTop:
                    cell.sr.sprite = environmentData.WallTop;
                    break;

                case GridCell.CellType.wallBottom:
                    cell.sr.sprite = environmentData.WallBottom;
                    break;

                case GridCell.CellType.wallLeft:
                    cell.sr.sprite = environmentData.WallLeft;
                    break;

                case GridCell.CellType.wallRight:
                    cell.sr.sprite = environmentData.WallRight;
                    break;

                case GridCell.CellType.cornerLeftTop:
                    cell.sr.sprite = environmentData.WallTopLeft;
                    break;

                case GridCell.CellType.cornerRightTop:
                    cell.sr.sprite = environmentData.WallTopRight;
                    break;

                case GridCell.CellType.cornerLeftBottom:
                    cell.sr.sprite = environmentData.WallBottomLeft;
                    break;

                case GridCell.CellType.cornerRightBottom:
                    cell.sr.sprite = environmentData.WallBottomRight;
                    break;

                case GridCell.CellType.corridorLeftTop:
                    cell.sr.sprite = environmentData.CorridorLeftTop;
                    break;

                case GridCell.CellType.corridorRightTop:
                    cell.sr.sprite = environmentData.CorridorRightTop;
                    break;

                case GridCell.CellType.corridorLeftBottom:
                    cell.sr.sprite = environmentData.CorridorLeftBottom;
                    break;

                case GridCell.CellType.corridorRightBottom:
                    cell.sr.sprite = environmentData.CorridorRightBottom;
                    break;
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

    public void SetCellsToCorrectWalls(Room room)
    {
        if (room == null || room.cells == null)
        {
            return;
        }

        RectInt bounds = room.bounds;

        foreach (GridCell cell in room.cells)
        {
            if (cell == null)
            {
                continue;
            }

            bool isLeft = cell.x == bounds.xMin;
            bool isRight = cell.x == bounds.xMax - 1;
            bool isBottom = cell.y == bounds.yMin;
            bool isTop = cell.y == bounds.yMax - 1;
            
            if (isLeft && isBottom)
            {
                cell.type = GridCell.CellType.cornerLeftBottom;
            }
            else if (isRight && isBottom)
            {
                cell.type = GridCell.CellType.cornerRightBottom;
            }
            else if (isLeft && isTop)
            {
                cell.type = GridCell.CellType.cornerLeftTop;
            }
            else if (isRight && isTop)
            {
                cell.type = GridCell.CellType.cornerRightTop;
            }
            else if (isLeft)
            {
                cell.type = GridCell.CellType.wallLeft;
            }
            else if (isRight)
            {
                cell.type = GridCell.CellType.wallRight;
            }
            else if (isBottom)
            {
                cell.type = GridCell.CellType.wallBottom;
            }
            else if (isTop)
            {
                cell.type = GridCell.CellType.wallTop;
            }
            else
            {
                cell.type = GridCell.CellType.floor;
            }
        }
    }

    public void FindAdjectedRooms(Room room, List<Room> allRooms)
    {
        if (room == null || allRooms == null)
        {
            return;
        }
        foreach (Room otherRoom in allRooms)
        {
            if (otherRoom == room)
            {
                continue;
            }
            if (room.bounds.xMax == otherRoom.bounds.xMin && room.bounds.yMin < otherRoom.bounds.yMax && room.bounds.yMax > otherRoom.bounds.yMin)
            {
                room.adjectedRooms[RoomsAdjected.Right] = otherRoom;
            }
            else if (room.bounds.xMin == otherRoom.bounds.xMax && room.bounds.yMin < otherRoom.bounds.yMax && room.bounds.yMax > otherRoom.bounds.yMin)
            {
                room.adjectedRooms[RoomsAdjected.Left] = otherRoom;
            }
            else if (room.bounds.yMax == otherRoom.bounds.yMin && room.bounds.xMin < otherRoom.bounds.xMax && room.bounds.xMax > otherRoom.bounds.xMin)
            {
                room.adjectedRooms[RoomsAdjected.Top] = otherRoom;
            }
            else if (room.bounds.yMin == otherRoom.bounds.yMax && room.bounds.xMin < otherRoom.bounds.xMax && room.bounds.xMax > otherRoom.bounds.xMin)
            {
                room.adjectedRooms[RoomsAdjected.Bottom] = otherRoom;
            }
        }
    }

    public void ConnectSmallerRooms()
    {
        Dictionary<Room, List<GridCell>> smallerRooms = new Dictionary<Room, List<GridCell>>();

        foreach (Room room in mapGenerator.allRoomList)
        {
            smallerRooms.Add(room, room.cells);
        }

        Debug.Log($"[Room] Found {smallerRooms.Count} rooms to check.");

        if (smallerRooms.Count <= 1)
        {
            return;
        }

        smallerRooms = smallerRooms.OrderBy(s => s.Value.Count).ToDictionary(s => s.Key, s => s.Value);

        int smallestRoomCellCount = smallerRooms.First().Value.Count;
        List<Room> smallestRoomsList = new List<Room>();

        Debug.Log($"[Room] Smallest room has {smallestRoomCellCount} cells.");

        foreach (var sRoom in smallerRooms)
        {
            if (sRoom.Value.Count == smallestRoomCellCount)
            {
                smallestRoomsList.Add(sRoom.Key);
            }
            else
            {
                break;
            }
        }

        Debug.Log($"[Room] Found {smallestRoomsList.Count} smallest rooms to connect.");

        while (smallestRoomsList.Count > 1)
        {
            Room currentRoom = smallestRoomsList[0];
            Room roomToConnect = null;

            foreach (Room otherRoom in smallestRoomsList)
            {
                if (otherRoom != currentRoom && AreRoomsAdjected(currentRoom, otherRoom))
                {
                    roomToConnect = otherRoom;
                    break;
                }
            }

            if (roomToConnect != null)
            {
                ConnectRooms(currentRoom, roomToConnect);

                smallestRoomsList.Remove(currentRoom);
                smallestRoomsList.Remove(roomToConnect);
            }
            else
            {
                smallestRoomsList.Remove(currentRoom);
            }
        }

        Debug.Log($"[Room] Connected smallest bordering rooms. Total rooms now: {mapGenerator.allRoomList.Count}"); 
    }

    public bool AreRoomsAdjected(Room room1, Room room2)
    {
        if (room1 == null || room2 == null)
        {
            return false;
        }
        return room1.adjectedRooms.ContainsValue(room2) || room2.adjectedRooms.ContainsValue(room1);
    }

    public void ConnectRooms(Room room1, Room room2)
    {
        if (room1 == null || room2 == null)
        {
            Debug.Log($"[Room] Cannot connect null rooms: room1 = {room1}, room2 = {room2}");
            return;
        }

        if (!AreRoomsAdjected(room1, room2))
        {
            Debug.LogWarning("[Room] Rooms are not adjected and cannot be connected.");
            return;
        }

        List<GridCell> combinedCells = new List<GridCell>(room1.cells);
        combinedCells.AddRange(room2.cells);

        Room newRoom = new Room("ConnectedRoom", combinedCells, room1.environmentData);

        foreach (GridCell cell in combinedCells)
        {
            cell.hasRoom = true;
            cell.roomOwner = newRoom;
        }

        mapGenerator.allRoomList.Remove(room1);
        mapGenerator.allRoomList.Remove(room2);
        mapGenerator.allRoomList.Add(newRoom);
    }

}
