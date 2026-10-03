
/*using System.Collections.Generic;
using System.Linq;

public List<Room, List<GridCell>> FindSmallestRooms()
{
    Dictionary<Room, List<GridCell>> smallerRooms = new Dictionary<Room, List<GridCell>>();

    foreach (Room room in mapGenerator.allRoomList)
    {
        smallerRooms.Add(room, room.cells);
    }

    

    smallerRooms = smallerRooms.OrderBy(s => s.Value.Count).ToDictionary(s => s.Key, s => s.Value);
    int smallestRoomCellCount = smallerRooms.First().Value.Count;
    List<Room> smallestRoomsList = new List<Room>();
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
    return smallestRoomsList;
}

public void ConnectSmallerRooms()
{
    canWork = true;
    while (canWork)
    {
        List<Room> smallestRooms = FindSmallestRooms();
        if (smallestRooms.Count < 2)
        {
            canWork = false;
            break;
        }
        Room currentRoom = smallestRooms[0];
        List<Room> roomToConnect = new List<Room>();

        foreach (Room otherRoom in smallestRooms)
        {
            if (otherRoom != currentRoom && AreRoomsAdjected(currentRoom, otherRoom))
            {
                roomToConnect.Add(otherRoom);
                break;
            }
        }

        foreach (Room room in roomToConnect)
        {
            int chance = Random.Range(0, 1);
            if (chance == 1)
            {
                ConnectRooms(currentRoom, room);
            }
            else
            {
                roomToConnect.Remove(room);
            }
        }
    }
}*/