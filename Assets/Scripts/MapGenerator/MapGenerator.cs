using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int iterations = 4;
    public int roomCount = 0;

    public int mapWidth;
    public int mapHeight;
    public Grid gridMap;
    public Room roomScript;
    public Room startRoom;
    public GameObject gridCellPref;
    public List<Room> allRoomList = new List<Room>();

    [SerializeField] private EnvironmentData[] roomEnvironment;

    private EnvironmentData[] notUsedRoomEnvironments;
    private List<Room> roomList;

    private GridCell[,] grid;
    private readonly List<GridCell> emptyCells = new List<GridCell>();

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    public void Start()
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
            EnvironmentData selectedStyle = AssignRandomStyleToRoom(room);

            if (selectedStyle != null)
            {
                roomScript.SetCellsToCorrectWalls(room);
                roomScript.SetRoomEnvironment(room, selectedStyle);
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

        
    }

    public void GenerateGrid(int width, int height)
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
