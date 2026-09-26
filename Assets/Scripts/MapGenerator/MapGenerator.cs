using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class MapGenerator : MonoBehaviour
{
    [Header("Scripts")]
    public static MapGenerator Instance { get; private set; }
    public DoorScript doorScript;
    public Room roomScript;

    [Header("Prefabs")]
    public GameObject gridCellPref;
    public GameObject doorPrefab;

    [Header("Map Settings")]
    public int iterations = 4;
    public int roomCount = 0;
    public int corridorCount = 0;
    public int smallRoomCellLimit = 100;
    public int mapWidth;
    public int mapHeight;
    public Grid gridMap;
    public Room startRoom;
    public List<Room> allRoomList = new List<Room>();
    public List<Corridor> corridorList = new List<Corridor>();
    private List<GridCell> cellsList = new List<GridCell>();

    [SerializeField] public EnvironmentData[] roomEnvironment;

    private EnvironmentData[] notUsedRoomEnvironments;
    private List<Room> roomList;

    public GridCell[,] grid;
    private readonly List<GridCell> emptyCells = new List<GridCell>();
    private List<Corridor> createdCorridors = new List<Corridor>();

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

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
        public Orientation orientation;

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
        GenerateRooms();

        GroupUpExistingRooms();

        CreateCorridorsBetweenRooms();
        CreateCorridorWalls(corridorList);
        CheckIfRoomsAreConnected();
        MakeEveryRoomHasEnoughCorridors();
        foreach (Room room in allRoomList)
        {
            if (room.environmentData != null)
            {
                roomScript.SetRoomEnvironment(room, room.environmentData);
            }
        }
        GiveCollidersToWalls();
    }

    #region ----- Map Generating -----

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
                cellsList.Add(newCell);
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
    private void GiveCollidersToWalls()
    {
        List<GridCell> wallsList = cellsList.Where(c => c.type != GridCell.CellType.floor)
            .ToList();

        foreach (GridCell wall in wallsList)
        {
            Sprite sprite = wall.GetComponent<SpriteRenderer>().sprite;
            var shapeCount = sprite.GetPhysicsShapeCount();
            List<Vector2> vectorList = new List<Vector2>();
            PolygonCollider2D col = wall.AddComponent<PolygonCollider2D>();
            for (int i = 0; i < shapeCount; i++)
            {
                var shape = sprite.GetPhysicsShape(i, vectorList);
                col.SetPath(i, vectorList);
                vectorList.Clear();
            }
        }
    }

    #endregion

    #region ----- Rooms -----
    public void GenerateRooms()
    {

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

            if (roomList.Count > 4)
            {
                int randomRoomCount = Random.Range(roomList.Count * 3 / 4, roomList.Count - 1);
                randomRoomList = roomList.OrderBy(_ => Random.value).Take(randomRoomCount).ToList();
                /*
                for (int j = 0; j < randomRoomCount; j++)
                {
                    int randomIndex = Random.Range(0, roomList.Count);
                    randomRoomList.Add(roomList[randomIndex]);
                    allRoomList.Remove(roomList[randomIndex]);
                }*/
                foreach (Room room in randomRoomList)
                {
                    allRoomList.Remove(room);
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
            EnvironmentData selectedStyle = AssignRandomStyleToRoom(room);

            if (selectedStyle != null)
            {
                roomScript.SetCellsToCorrectWalls(room);

                room.environmentData = selectedStyle;
                foreach (var cell in room.cells)
                {
                    cell.environmentData = selectedStyle;
                }

                //Debug.Log($"Room {room.roomName} environment set to: {selectedStyle.name}");
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

    private Dictionary<int, List<Room>> GroupUpExistingRooms()
    {
        Dictionary<Room, List<GridCell>> allRooms = new Dictionary<Room, List<GridCell>>();
        foreach (var room in allRoomList)
        {
            allRooms.Add(room, room.cells);
        }
        allRooms = allRooms.OrderBy(s => s.Value.Count).ToDictionary(s => s.Key, s => s.Value);
        List<int> roomSizes = new List<int>();
        foreach (var sRoom in allRooms)
        {
            if (!roomSizes.Contains(sRoom.Value.Count))
            {
                roomSizes.Add(sRoom.Value.Count);
            }
        }

        Dictionary<int, List<Room>> roomsBySize = new Dictionary<int, List<Room>>();
        foreach (int size in roomSizes)
        {
            roomsBySize[size] = allRooms.Where(s => s.Value.Count == size).Select(s => s.Key).ToList();
            Debug.Log("[Room] Found " + roomsBySize[size].Count + " rooms of size " + size);
        }
        Debug.Log("[Room] Grouped up existing rooms by size");
        return roomsBySize;
    }

    private void DeleteSomeRooms()
    {
        var allRooms = GroupUpExistingRooms();
        List<int> roomKeys = allRooms.Keys.ToList();
        roomKeys = roomKeys.OrderBy(s => s).ToList();
        Debug.Log("[Room] Room sizes in ascending order: " + string.Join(", ", roomKeys));
        int deletedRoomsCount = 0;
        foreach (var roomGroup in allRooms)
        {
            if (roomGroup.Key <= roomKeys[3])
            {
                if (roomGroup.Value.Count >= 5)
                {
                    int roomsToDeleteCount = roomGroup.Value.Count / 2;
                    Debug.Log("[Room] Deleting " + roomsToDeleteCount + " rooms of size " + roomGroup.Key);
                    for (int i = 0; i < roomsToDeleteCount; i++)
                    {
                        deletedRoomsCount++;
                        Room roomToDelete = roomGroup.Value[i];
                        foreach (var cell in roomToDelete.cells)
                        {
                            cell.type = GridCell.CellType.floor;
                            cell.environmentData = roomEnvironment[0];
                            if (cell.sr != null)
                            {
                                cell.sr.sprite = roomEnvironment[0].Floor;
                            }
                        }
                        allRoomList.Remove(roomToDelete);
                        Debug.Log("[Room] Deleted room: " + roomToDelete.roomName);
                    }
                }
            }
        }
        Debug.Log($"[Room] Deleted a total of {deletedRoomsCount} rooms");
    }

    #endregion

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
            int minY1 = room1.bounds.yMin;
            int maxY1 = room1.bounds.yMax - 1;
            int minY2 = room2.bounds.yMin;
            int maxY2 = room2.bounds.yMax - 1;

            room1PossibbleCorridorCells.RemoveAll(cell => cell.y - 1 <= minY1 || cell.y >= maxY2 || cell.y - 1 <= minY2 || cell.y >= maxY1);
        }
        else if (direction == Room.RoomsAdjected.Top || direction == Room.RoomsAdjected.Bottom)
        {
            int minX1 = room1.bounds.xMin;
            int maxX1 = room1.bounds.xMax - 1;
            int minX2 = room2.bounds.xMin;
            int maxX2 = room2.bounds.xMax - 1;

            room1PossibbleCorridorCells.RemoveAll(cell => cell.x <= minX1 || cell.x + 1 >= maxX1 || cell.x - 1 <= minX2 || cell.x + 1>= maxX2);
        }

        if (room1PossibbleCorridorCells.Count == 0)
        {
            Debug.LogWarning("No valid corridor cells found.");
            return false;
        }

        GridCell corridorStart = room1PossibbleCorridorCells[Random.Range(0, room1PossibbleCorridorCells.Count)];
        Corridor corridor = new Corridor();
        createdCorridors.Add(corridor);
        corridor.room1 = room1;
        corridor.room2 = room2;
        room1.OwnedCorridors.Add(corridor);
        room2.OwnedCorridors.Add(corridor);

        switch (direction)
        {
            case Room.RoomsAdjected.Left:
                corridor.rightTopCell = corridorStart;
                corridor.rightBottomCell = room1.cells.FirstOrDefault(c => c.x == corridorStart.x && c.y == corridorStart.y - 1);
                corridor.leftTopCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x - 1 && c.y == corridorStart.y);
                corridor.leftBottomCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x - 1 && c.y == corridorStart.y - 1);
                corridor.orientation = Corridor.Orientation.Horizontal;
                break;
            case Room.RoomsAdjected.Right:
                corridor.leftTopCell = corridorStart;
                corridor.leftBottomCell = room1.cells.FirstOrDefault(c => c.x == corridorStart.x && c.y == corridorStart.y - 1);
                corridor.rightTopCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x + 1 && c.y == corridorStart.y);
                corridor.rightBottomCell = room2.cells.FirstOrDefault(c => c.x == corridorStart.x + 1 && c.y == corridorStart.y - 1);
                corridor.orientation = Corridor.Orientation.Horizontal;
                break;
            case Room.RoomsAdjected.Top:
                corridor.leftBottomCell = corridorStart;
                corridor.rightBottomCell = room1.cells.FirstOrDefault(c => c.y == corridorStart.y && c.x == corridorStart.x + 1);
                corridor.leftTopCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y + 1 && c.x == corridorStart.x);
                corridor.rightTopCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y + 1 && c.x == corridorStart.x + 1);
                corridor.orientation = Corridor.Orientation.Vertical;
                break;
            case Room.RoomsAdjected.Bottom:
                corridor.leftTopCell = corridorStart;
                corridor.rightTopCell = room1.cells.FirstOrDefault(c => c.y == corridorStart.y && c.x == corridorStart.x + 1);
                corridor.leftBottomCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y - 1 && c.x == corridorStart.x);
                corridor.rightBottomCell = room2.cells.FirstOrDefault(c => c.y == corridorStart.y - 1 && c.x == corridorStart.x + 1);
                corridor.orientation = Corridor.Orientation.Vertical;
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

        corridor.leftTopCell.type = GridCell.CellType.floor;
        corridor.leftBottomCell.type = GridCell.CellType.floor;
        corridor.rightTopCell.type = GridCell.CellType.floor;
        corridor.rightBottomCell.type = GridCell.CellType.floor;

        foreach (var cell in corridor.corridorCells)
        {
            //Debug.Log($"[Corridor] Corridor cell at ({cell.x}, {cell.y}) of type {cell.type}");
            var cellNeighbors = GetCellNeighbors(cell);
            foreach (var kvp in cellNeighbors)
            {
                var cellDirection = kvp.Key;
                var neighbor = kvp.Value;

                if(neighbor == null)
                {
                    Debug.Log("[Cell] missing neighbor for cell at (" + cell.x + ", " + cell.y + ") in direction " + cellDirection);
                }
                
            }
        }
        corridorList.Add(corridor);

        return true;
    }

    public Dictionary<Directions, GridCell> GetCellNeighbors(GridCell cell)
    {
        Dictionary<Directions, GridCell> neighbors = new Dictionary<Directions, GridCell>();

        neighbors.Add(Directions.Up, cellsList.FirstOrDefault(c => c.x == cell.x && c.y == cell.y + 1));
        neighbors.Add(Directions.Down, cellsList.FirstOrDefault(c => c.x == cell.x && c.y == cell.y - 1));
        neighbors.Add(Directions.Left, cellsList.FirstOrDefault(c => c.x == cell.x - 1 && c.y == cell.y));
        neighbors.Add(Directions.Right, cellsList.FirstOrDefault(c => c.x == cell.x + 1 && c.y == cell.y));

        return neighbors;
    }

    public void CreateCorridorWalls(List<Corridor> corridors)
    {
        foreach (var corridor in corridors)
        {
            //Checking if a wall next to corridor wall belongs to the same room as corridor wall
            if (corridor.orientation == Corridor.Orientation.Horizontal)
            {
                var leftTopWall = cellsList.FirstOrDefault(c => c.x == corridor.leftTopCell.x && c.y == corridor.leftTopCell.y + 1);
                var leftBottomWall = cellsList.FirstOrDefault(c => c.x == corridor.leftBottomCell.x && c.y == corridor.leftBottomCell.y - 1);
                var rightTopWall = cellsList.FirstOrDefault(c => c.x == corridor.rightTopCell.x && c.y == corridor.rightTopCell.y + 1);
                var rightBottomWall = cellsList.FirstOrDefault(c => c.x == corridor.rightBottomCell.x && c.y == corridor.rightBottomCell.y - 1);

                //LeftTop
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftTopWall.x && c.y == leftTopWall.y + 1)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftTopWall.x && c.y == leftTopWall.y + 1)))
                {
                    leftTopWall.type = GridCell.CellType.corridorRightTop;
                }
                else
                {
                    leftTopWall.type = GridCell.CellType.wallTop;
                }
                Debug.Log($"[CORRIDOR] - {leftTopWall.type} created at {leftTopWall.x}, {leftTopWall.y}");
                //RightTop
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightTopWall.x && c.y == rightTopWall.y + 1)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightTopWall.x && c.y == rightTopWall.y + 1)))
                {
                    rightTopWall.type = GridCell.CellType.corridorLeftTop;
                }
                else
                {
                    rightTopWall.type = GridCell.CellType.wallTop;
                }

                //LeftBottom
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftBottomWall.x && c.y == leftBottomWall.y - 1)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftBottomWall.x && c.y == leftBottomWall.y - 1)))
                {
                    leftBottomWall.type = GridCell.CellType.corridorRightBottom;
                }
                else
                {
                    leftBottomWall.type = GridCell.CellType.wallBottom;
                }
                //RightBottom
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightBottomWall.x && c.y == rightBottomWall.y - 1)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightBottomWall.x && c.y == rightBottomWall.y - 1)))
                {
                    rightBottomWall.type = GridCell.CellType.corridorLeftBottom;
                }
                else
                {
                    rightBottomWall.type = GridCell.CellType.wallBottom;
                }

            }
            else if (corridor.orientation == Corridor.Orientation.Vertical)
            // In this one, due to orientation, the name of the wall will be opposite bi-directional than the cell position to match the rest of the walls
            {
                var leftTopWall = cellsList.FirstOrDefault(c => c.x == corridor.leftTopCell.x - 1 && c.y == corridor.leftTopCell.y);
                var leftBottomWall = cellsList.FirstOrDefault(c => c.x == corridor.leftBottomCell.x - 1 && c.y == corridor.leftBottomCell.y);
                var rightTopWall = cellsList.FirstOrDefault(c => c.x == corridor.rightTopCell.x + 1 && c.y == corridor.rightTopCell.y);
                var rightBottomWall = cellsList.FirstOrDefault(c => c.x == corridor.rightBottomCell.x + 1 && c.y == corridor.rightBottomCell.y);

                //LeftTop
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftTopWall.x - 1 && c.y == leftTopWall.y)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftTopWall.x - 1 && c.y == leftTopWall.y)))
                {
                    leftTopWall.type = GridCell.CellType.corridorLeftBottom;
                }
                else
                {
                    leftTopWall.type = GridCell.CellType.wallLeft;
                }

                //RightTop
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightTopWall.x + 1 && c.y == rightTopWall.y)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightTopWall.x + 1 && c.y == rightTopWall.y)))
                {
                    rightTopWall.type = GridCell.CellType.corridorRightBottom;
                }
                else
                {
                    rightTopWall.type = GridCell.CellType.wallRight;
                }

                //LeftBottom
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftBottomWall.x - 1 && c.y == leftBottomWall.y)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == leftBottomWall.x - 1 && c.y == leftBottomWall.y)))
                {
                    leftBottomWall.type = GridCell.CellType.corridorLeftTop;
                }
                else
                {
                    leftBottomWall.type = GridCell.CellType.wallLeft;
                }

                //RightBottom
                if (corridor.room1.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightBottomWall.x + 1 && c.y == rightBottomWall.y)) ||
                    corridor.room2.cells.Contains(cellsList.FirstOrDefault(c => c.x == rightBottomWall.x + 1 && c.y == rightBottomWall.y)))
                {
                    rightBottomWall.type = GridCell.CellType.corridorRightTop;
                }
                else
                {
                    rightBottomWall.type = GridCell.CellType.wallRight;
                }
            }
        }
    }
    private void CheckIfRoomsAreConnected()
    {
        List<Room> CheckedRooms = new List<Room>();
        //iterating as long as all rooms are connected
        var randomRoom = allRoomList[Random.Range(0, allRoomList.Count)];
        while (CheckedRooms.Count < allRoomList.Count)
        {
            Queue<Room> roomsToCheck = new Queue<Room>();
            roomsToCheck.Enqueue(randomRoom);
            Debug.Log("[RoomChecker] is about to start");
            CheckedRooms.Clear();
            while (roomsToCheck.Count != 0)
            {
                var currentRoom = roomsToCheck.Peek();
                var ownedCorridors = currentRoom.OwnedCorridors;
                foreach (var corridor in ownedCorridors)
                {
                    Room roomToQueue;
                    if (corridor.room1 != currentRoom)
                    {
                        roomToQueue = corridor.room1;
                    }
                    else
                    {
                        roomToQueue = corridor.room2;
                    }

                    if (!CheckedRooms.Contains(roomToQueue))
                    {
                        Debug.Log($"[RoomChecker] - {roomToQueue.roomName} will be added to the queue ");
                        roomsToCheck.Enqueue(roomToQueue);
                    }
                }
                CheckedRooms.Add(currentRoom);
                var finishedRoom = roomsToCheck.Dequeue();
                Debug.Log($"[RoomChecker] - Dequeued {finishedRoom.roomName}");
            }
            Debug.Log($"[RoomChecker] - Queue Finished, checked {CheckedRooms.Count} out of {allRoomList.Count} rooms");

            List<Room> roomsToConnectSomehow = new List<Room>();
            roomsToConnectSomehow = allRoomList.Except(CheckedRooms).ToList();
            Debug.Log($"[RoomChecker] - Rooms to connect somehow: {roomsToConnectSomehow.Count}");

            Room newRoom = roomsToConnectSomehow
                .Where(x => x.adjectedRooms.Values.Any(y => CheckedRooms.Contains(y)))
                .FirstOrDefault();
            if (newRoom == null)
            {
                Debug.LogWarning($"[RoomChecker] - No new room found to connect, but there are still {roomsToConnectSomehow.Count} rooms left to connect. This might indicate a problem with the room adjacency.");
                break;
            }
            Room newRoomGoodNeighbour = newRoom.adjectedRooms.Values.FirstOrDefault(x => CheckedRooms.Contains(x));
            CreateCorridor(newRoom, newRoomGoodNeighbour);
            if (newRoomGoodNeighbour == null)
            {
                Debug.LogWarning($"[RoomChecker] - No good neighbour found for {newRoom.roomName}. This might indicate a problem with the room adjacency.");
                break;
            }
        }
        Debug.Log($"[RoomChecker] - All rooms has been connected onto one building");
    }

    private void MakeEveryRoomHasEnoughCorridors()
    {
        List<Room> roomsWithNotEnoughCorridors = allRoomList.Where(x => x.roomsConnectedWithCorridor < 2).ToList();
        foreach (var room in roomsWithNotEnoughCorridors)
        {
            TryCreateCorridorForRoom(room, 3);
        }
    }


    #endregion

    #region ----- Doors -----

    private void CreateDoors()
    {
        List<Corridor> corridors = corridorList;
        foreach (var corridor in corridors)
        {
            GameObject newDoor = Instantiate(doorPrefab);
            var newDoorScript = newDoor.GetComponent<DoorScript>();
            newDoorScript.mapGenerator = this;
            newDoorScript.belongedCorridor = corridor;

        }
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
