using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.Cinemachine;

public class MapGenerator : MonoBehaviour
{
    public int iterations = 4;
    public int roomCount = 0;
    public int corridorCount = 0;

    public int mapWidth;
    public int mapHeight;
    public Grid gridMap;
    public Room roomScript;
    public Room startRoom;
    public GameObject gridCellPref;
    public List<Room> allRoomList = new List<Room>();
    public List<Corridor> corridorList = new List<Corridor>();


    [SerializeField] protected EnvironmentData[] roomEnvironment;

    private EnvironmentData[] notUsedRoomEnvironments;
    private List<Room> roomList;

    protected GridCell[,] grid;
    private readonly List<GridCell> emptyCells = new List<GridCell>();

    // Dostep do siatki dla systemu szukania sciezki (GridAdapter).
    public GridCell[,] Grid => grid;

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }
    public class Corridor
    {
        public Room room1;
        public Room room2;

        public GridCell leftTopCell;
        public GridCell rightTopCell;
        public GridCell leftBottomCell;
        public GridCell rightBottomCell;
        public List<GridCell> corridorCells = new List<GridCell>();

    }


    public virtual void Start()
    {
        if (roomEnvironment == null || roomEnvironment.Length == 0 || roomEnvironment[0] == null)
        {
            Debug.LogError("[MapGenerator] Missing room environments.");
            return;
        }

        if (gridCellPref == null)
        {
            Debug.LogError("[MapGenerator] Missing gridCellPref.");
            return;
        }

        roomScript = new Room("Room_Manager", new List<GridCell>(), null);
        roomScript.mapGenerator = this;
        roomList = new List<Room>();
        notUsedRoomEnvironments = roomEnvironment;

        GenerateGrid(mapWidth, mapHeight);

        allRoomList = new List<Room>(roomList);

        //iteration for creating rooms
        for (int i = 0; i < iterations; i++)
        {
            if (roomList.Count == 0)
            {
                break;
            }

            List<Room> nextRoomList = new List<Room>();
            List<Room> randomRoomList = new List<Room>();

            if (roomList.Count >  4 )
            {
                int randomRoomCount = Random.Range(roomList.Count*3 / 4, roomList.Count - 1);
                for (int j = 0; j < randomRoomCount; j++)
                {
                    int randomIndex = Random.Range(0, roomList.Count);
                    randomRoomList.Add(roomList[randomIndex]);
                    allRoomList.Remove(roomList[randomIndex]);
                }
            }
            else
            {
                randomRoomList = roomList;
            }

            foreach (Room room in randomRoomList)
            {
                (Room room1, Room room2) = roomScript.SeparateRoom(room);

                if (room1 != null && room2 != null)
                {
                    nextRoomList.Add(room1);
                    nextRoomList.Add(room2);

                    allRoomList.Remove(room);
                    allRoomList.Add(room1);
                    allRoomList.Add(room2);
                }
                else
                {
                    Debug.LogWarning($"Room separation failed for {room.roomName}. Keeping original room.");
                    nextRoomList.Add(room);
                }
            }

            
            roomList = nextRoomList;
        }

        foreach (var room in allRoomList)
        {
            roomScript.FindAdjectedRooms(room, allRoomList);
        }

        roomCount = roomList.Count;
        roomScript.ConnectSmallerRooms();

        foreach (Room room in allRoomList)
        {
            room.adjectedRooms.Clear();
        }

        foreach (Room room in allRoomList)
        {
            roomScript.FindAdjectedRooms(room, allRoomList);
        }

        roomCount = allRoomList.Count;

        //Assigning environment
        foreach (Room room in allRoomList)
        {
            AssignEnvironment(room);
        }

        CreateCorridorsBetweenRooms();
        corridorCount = corridorList.Count;

        foreach (Room room in allRoomList)
        {
            if (room.environmentData != null)
            {
                roomScript.SetRoomEnvironment(room, room.environmentData);
            }
        }
    }

    public void AssignEnvironment (Room room)
    {
         EnvironmentData selectedStyle = AssignRandomStyleToRoom(room);

            if (selectedStyle != null)
            {
                roomScript.SetCellsToCorrectWalls(room);

                room.environmentData = selectedStyle;
                foreach (var cell in room.cells)
                {
                    cell.environmentData = selectedStyle;
                }

                Debug.Log($"Room {room.roomName} environment set to: {selectedStyle.name}");
                Color randomColor = GetRandomColor();
                foreach (var cell in room.cells)
                {
                    if (cell.sr != null)
                    {
                        cell.sr.color = randomColor;
                    }
                }
            }            
    }

    public virtual void GenerateGrid(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;
        grid = new GridCell[width, height];
        emptyCells.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newCellObject = Instantiate(gridCellPref, new Vector3(x, y, 0), Quaternion.identity);
                newCellObject.transform.SetParent(transform);

                GridCell newCell = newCellObject.GetComponent<GridCell>();

                if (newCell == null)
                {
                    Debug.LogError("[MapGenerator] gridCellPref does not contain GridCell component.");
                    continue;
                }

                newCell.x = x;
                newCell.y = y;
                newCell.type = GridCell.CellType.floor;
                newCell.environmentData = roomEnvironment[0];

                if (newCell.sr == null)
                {
                    newCell.sr = newCellObject.GetComponent<SpriteRenderer>();
                }

                if (newCell.sr != null)
                {
                    newCell.sr.sprite = roomEnvironment[0].Floor;
                }

                grid[x, y] = newCell;
                emptyCells.Add(newCell);
            }
        }

        Debug.Log($"Grid generated with dimensions: {width}x{height}. Cell count: {emptyCells.Count}");

        startRoom = roomScript.CreateNewRoom(emptyCells);

        if (startRoom != null)
        {
            roomList.Add(startRoom);
            Debug.Log($"startRoom created: {startRoom.roomName}");
        }
    }

    private EnvironmentData AssignRandomStyleToRoom(Room room)
    {
        if (notUsedRoomEnvironments.Length == 0)
        {
            Debug.LogWarning("[MapGenerator] No more unused room environments available.");
            return null;
        }
        int randomIndex = Random.Range(0, notUsedRoomEnvironments.Length);
        EnvironmentData selectedEnvironment = notUsedRoomEnvironments[randomIndex];
        /*
        List<EnvironmentData> notUsedRooms = new List<EnvironmentData>(notUsedRoomEnvironments);
        notUsedRooms.RemoveAt(randomIndex);
        notUsedRoomEnvironments = notUsedRooms.ToArray();*/
        return selectedEnvironment;
    }
    #region ----- Corridors -----
    public void CreateCorridorsBetweenRooms()
    {
        foreach (Room room in allRoomList.OrderBy(_ => Random.value))
        {
            if (room.roomsConnectedWithCorridor == 0)
            {
                TryCreateCorridorForRoom(room, 1);
            }
        }

        foreach (Room room in allRoomList.OrderBy(_ => Random.value))
        {
            if (room.roomsConnectedWithCorridor == 0)
            {
                TryCreateCorridorForRoom(room, 2);
            }
        }

        foreach (Room room in allRoomList.OrderBy(_ => Random.value))
        {
            if (room.roomsConnectedWithCorridor < 2)
            {
                TryCreateCorridorForRoom(room, 2);
            }
        }
    }

    private bool TryCreateCorridorForRoom(Room currentRoom, int maxConnections)
    {
        if (currentRoom.adjectedRooms.Count == 0 || currentRoom.roomsConnectedWithCorridor >= maxConnections)
        {
            return false;
        }

        List<Room> potentialRoomsToConnect = currentRoom.adjectedRooms.Values
            .Where(room => room != null &&
                           room.roomsConnectedWithCorridor < maxConnections &&
                           !CorridorAlreadyExists(currentRoom, room))
            .OrderBy(_ => Random.value)
            .ToList();

        foreach (Room roomToConnect in potentialRoomsToConnect)
        {
            if (CreateCorridor(currentRoom, roomToConnect))
            {
                currentRoom.roomsConnectedWithCorridor++;
                roomToConnect.roomsConnectedWithCorridor++;
                return true;
            }
        }

        return false;
    }

    private bool CorridorAlreadyExists(Room room1, Room room2)
    {
        return corridorList.Any(corridor =>
            (corridor.room1 == room1 && corridor.room2 == room2) ||
            (corridor.room1 == room2 && corridor.room2 == room1));
    }

    public bool CreateCorridor(Room room1, Room room2)
    {
        Room.RoomsAdjected? direction = null;
        foreach (var room in room1.adjectedRooms)
        {
            if (room.Value == room2)
            {
                direction = room.Key;
                break;
            }
        }

        if (direction == null)
        {
            Debug.LogWarning("Rooms are not adjacent.");
            return false;
        }

        List<GridCell> room1PossibbleCorridorCells = new List<GridCell>();        

        switch (direction)
        {
            case Room.RoomsAdjected.Left:
                foreach (var cell in room1.cells)
                {
                    if (cell.x == room1.cells.Min(c => c.x))
                    {
                        if (room2.cells.Any(c => c.x == cell.x - 1 && c.y == cell.y))
                        {
                            room1PossibbleCorridorCells.Add(cell);
                        }
                    }
                }
                break;
            case Room.RoomsAdjected.Right:
                foreach (var cell in room1.cells)
                {
                    if (cell.x == room1.cells.Max(c => c.x))
                    {
                        if (room2.cells.Any(c => c.x == cell.x + 1 && c.y == cell.y))
                        {
                            room1PossibbleCorridorCells.Add(cell);
                        }
                    }
                }
                break;
            case Room.RoomsAdjected.Top:
                foreach (var cell in room1.cells)
                {
                    if (cell.y == room1.cells.Max(c => c.y))
                    {
                        if (room2.cells.Any(c => c.y == cell.y + 1 && c.x == cell.x))
                        {
                            room1PossibbleCorridorCells.Add(cell);
                        }
                    }
                }
                break;
            case Room.RoomsAdjected.Bottom:
                foreach (var cell in room1.cells)
                {
                    if (cell.y == room1.cells.Min(c => c.y))
                    {
                        if (room2.cells.Any(c => c.y == cell.y - 1 && c.x == cell.x))
                        {
                            room1PossibbleCorridorCells.Add(cell);
                        }
                    }
                }
                break;
            default:
                Debug.LogWarning("Unexpected room adjacency.");
                return false;
        }

        if (direction == Room.RoomsAdjected.Left || direction == Room.RoomsAdjected.Right)
        {
            int minY = room1.cells.Min(c => c.y);
            int maxY = room1.cells.Max(c => c.y);

            room1PossibbleCorridorCells.RemoveAll(cell => cell.y == minY || cell.y == maxY);
        }
        else if (direction == Room.RoomsAdjected.Top || direction == Room.RoomsAdjected.Bottom)
        {
            int minX = room1.cells.Min(c => c.x);
            int maxX = room1.cells.Max(c => c.x);

            room1PossibbleCorridorCells.RemoveAll(cell => cell.x == minX || cell.x == maxX);
        }

        if (room1PossibbleCorridorCells.Count == 0)
        {
            Debug.LogWarning("No valid corridor cells found.");
            return false;
        }

        GridCell corridorStart = room1PossibbleCorridorCells[Random.Range(0, room1PossibbleCorridorCells.Count)];
        Corridor corridor = new Corridor();
        corridor.room1 = room1;
        corridor.room2 = room2;

        switch (direction)
        {
            case Room.RoomsAdjected.Left:
                corridor.rightTopCell = corridorStart;
                corridor.rightBottomCell = room1.cells.FirstOrDefault(c => c.x == corridorStart.x && c.y == corridorStart.y - 1);
                corridor.leftTopCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x - 1 && c.y == corridorStart.y);
                corridor.leftBottomCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x - 1 && c.y == corridorStart.y - 1);
                break;
            case Room.RoomsAdjected.Right:
                corridor.leftTopCell = corridorStart;
                corridor.leftBottomCell = room1.cells.FirstOrDefault(c => c.x == corridorStart.x && c.y == corridorStart.y - 1);
                corridor.rightTopCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x + 1 && c.y == corridorStart.y);
                corridor.rightBottomCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x + 1 && c.y == corridorStart.y - 1);
                break;
            case Room.RoomsAdjected.Top:
                corridor.leftBottomCell = corridorStart;
                corridor.rightBottomCell = room1.cells.FirstOrDefault(c => c.y == corridorStart.y && c.x == corridorStart.x + 1);
                corridor.leftTopCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y + 1 && c.x == corridorStart.x);
                corridor.rightTopCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y + 1 && c.x == corridorStart.x + 1);
                break;
            case Room.RoomsAdjected.Bottom:
                corridor.leftTopCell = corridorStart;
                corridor.rightTopCell = room1.cells.FirstOrDefault(c => c.y == corridorStart.y && c.x == corridorStart.x + 1);
                corridor.leftBottomCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y - 1 && c.x == corridorStart.x);
                corridor.rightBottomCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y - 1 && c.x == corridorStart.x + 1);
                break;
        }

        if (corridor.leftTopCell == null ||
            corridor.leftBottomCell == null ||
            corridor.rightTopCell == null ||
            corridor.rightBottomCell == null)
        {
            Debug.LogWarning("Corridor could not be created because one or more corridor cells were missing.");
            return false;
        }

        corridor.corridorCells.Add(corridor.leftTopCell);
        corridor.corridorCells.Add(corridor.leftBottomCell);
        corridor.corridorCells.Add(corridor.rightBottomCell);
        corridor.corridorCells.Add(corridor.rightTopCell);

        corridor.leftTopCell.type = GridCell.CellType.corridorLeftTop;
        corridor.leftBottomCell.type = GridCell.CellType.corridorLeftBottom;
        corridor.rightTopCell.type = GridCell.CellType.corridorRightTop;
        corridor.rightBottomCell.type = GridCell.CellType.corridorRightBottom;
        corridorList.Add(corridor);

        return true;
    }



#endregion

    #region --- Debugging and Visualization ---

    private Color GetRandomColor()
    {
        int randomRed = Random.Range(0, 256);
        int randomGreen = Random.Range(0, 256);
        int randomBlue = Random.Range(0, 256);

        return new Color(randomRed / 255f, randomGreen / 255f, randomBlue / 255f);
    }

    #endregion

}
