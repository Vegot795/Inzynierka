using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public string roomName;
    public RectInt bounds;
    public EnvironmentData environmentData;
    public List<GridCell> cells = new List<GridCell>();
    public List<Room> createdRooms = new List<Room>();

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
        List<GridCell> roomCells = room.cells;
        int roomBoundsWidth = room.bounds.width;
        int roomBoundsHeight = room.bounds.height;

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

    public List<GridCell> GetRoomWallCells(Room room)
    {
        List<GridCell> roomCells = room.cells;
        List<GridCell> wallCells = new List<GridCell>();

        var startCell = FindRoomStartCell(room);
        wallCells.Add(startCell);

        foreach (GridCell cell in roomCells)
        {
            if((cell.x == startCell.x || cell.y == startCell.y) && !wallCells.Contains(cell))
            {
                wallCells.Add(cell);
            }

            if((cell.x == room.bounds.x || cell.x == room.bounds.xMax - 1 || cell.y == room.bounds.y || cell.y == room.bounds.yMax - 1) && !wallCells.Contains(cell))
            {
                wallCells.Add(cell);
            }            
        }

        int predictedCellCount = (2 * room.bounds.width) + (2 * room.bounds.height) - 4;
        try
        {
            if (wallCells.Count == predictedCellCount)
            {
                foreach (GridCell cell in wallCells)
                {
                    cell.type = GridCell.CellType.empty;
                }
                return wallCells;
            }
            else
            {
                return null;
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Error calculating wall cells for room: " + room.roomName + ". Expected: " + predictedCellCount + ", Found: " + wallCells.Count);
            Debug.LogError(e.Message);
            return null;
        }
    }

    private (List<GridCell>, List<GridCell>, List<GridCell>, List<GridCell>, GridCell, GridCell, GridCell,GridCell) GetRoomWallCellsByDirection(Room room)
    {
        List<GridCell> roomCells = room.cells;
        List<GridCell> leftWallCells = new List<GridCell>();
        List<GridCell> rightWallCells = new List<GridCell>();
        List<GridCell> topWallCells = new List<GridCell>();
        List<GridCell> bottomWallCells = new List<GridCell>();
        GridCell topLeftCell = null;
        GridCell bottomLeftCell = null;
        GridCell topRightCell = null;
        GridCell bottomRightCell = null;

        foreach (GridCell cell in roomCells)
        {
            if (cell == null) continue;

            // Check for corner cells
            if (cell.x == room.bounds.x && cell.y == room.bounds.yMax - 1)
            {
                topLeftCell = cell;
                cell.type = GridCell.CellType.cornerLeftTop;
            }
            else if (cell.x == room.bounds.x && cell.y == room.bounds.y)
            {
                bottomLeftCell = cell;
                cell.type = GridCell.CellType.cornerLeftBottom;
            }
            else if (cell.x == room.bounds.xMax - 1 && cell.y == room.bounds.yMax - 1)
            {
                topRightCell = cell;
                cell.type = GridCell.CellType.cornerRightTop;
            }
            else if (cell.x == room.bounds.xMax - 1 && cell.y == room.bounds.y)
            {
                bottomRightCell = cell;
                cell.type = GridCell.CellType.cornerRightBottom;
            }

            //Check for wall cells
            if (cell.x == room.bounds.x)
            {
                leftWallCells.Add(cell);
                cell.type = GridCell.CellType.wallLeft;
            }
            else if (cell.x == room.bounds.xMax - 1)
            {
                rightWallCells.Add(cell);
                cell.type = GridCell.CellType.wallRight;
            }
            else if (cell.y == room.bounds.yMax - 1)
            {
                topWallCells.Add(cell);
                cell.type = GridCell.CellType.wallTop;
            }
            else if (cell.y == room.bounds.y)
            {
                bottomWallCells.Add(cell);
                cell.type = GridCell.CellType.wallBottom;
            }
        }
        return (leftWallCells, rightWallCells, topWallCells, bottomWallCells, topLeftCell, topRightCell, bottomLeftCell, bottomRightCell);
    }

    private GridCell FindRoomStartCell(Room room)
    {
        List<GridCell> roomCells = room.cells;
        GridCell randomCell = roomCells[Random.Range(0, roomCells.Count)];
        int lowestX = randomCell.x;
        int lowestY = randomCell.y;

        foreach(GridCell cell in roomCells)
        {
            if (cell.x < lowestX || (cell.x == lowestX && cell.y < lowestY))
            {
                lowestX = cell.x;
                lowestY = cell.y;
            }
        }

        return roomCells.Find(cell => cell.x == lowestX && cell.y == lowestY);

    }

    #region ----- sprites -----
    public void SetRoomEnvironment(Room room, EnvironmentData environmentData)
    {
        if (room == null || environmentData == null)
        {
            return;
        }

        room.environmentData = environmentData;
        room.SetCellSpritesFromEnvironment(room, environmentData);

        foreach (GridCell cell in room.cells)
        {
            cell.environmentData = environmentData;

            if (cell.sr != null)
            {
                cell.sr.sprite = environmentData.Floor;
            }
        }
    }

    private void SetCellSpritesFromEnvironment(Room room, EnvironmentData environmentData)
    {
        if (room == null || environmentData == null)
        {
            return;
        }
        List<GridCell> roomCells = room.cells;
        foreach (GridCell cell in roomCells)
        {
            if (cell.sr == null)
            {
                return;
            }

            if(cell.type == GridCell.CellType.floor)
            {
                cell.sr.sprite = environmentData.Floor;
            }
            else if(cell.type == GridCell.CellType.wallTop)
            {
                cell.sr.sprite = environmentData.WallTop;
            }
            else if(cell.type == GridCell.CellType.wallBottom)
            {
                cell.sr.sprite = environmentData.WallBottom;
            }
            else if(cell.type == GridCell.CellType.wallLeft)
            {
                cell.sr.sprite = environmentData.WallLeft;
            }
            else if(cell.type == GridCell.CellType.wallRight)
            {
                cell.sr.sprite = environmentData.WallRight;
            }
            else if(cell.type == GridCell.CellType.cornerLeftTop)
            {
                cell.sr.sprite = environmentData.WallTopLeft;
            }
            else if(cell.type == GridCell.CellType.cornerRightTop)
            {
                cell.sr.sprite = environmentData.WallTopRight;
            }
            else if(cell.type == GridCell.CellType.cornerLeftBottom)
            {
                cell.sr.sprite = environmentData.WallBottomLeft;
            }
            else if(cell.type == GridCell.CellType.cornerRightBottom)
            {
                cell.sr.sprite = environmentData.WallBottomRight;}
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

    #endregion
}
