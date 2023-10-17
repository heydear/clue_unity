using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<PlayerPlacedCheck> roomList;



    public PlayerPlacedCheck FindRoom(ROOM targetRoom)
    {
        return roomList?.Find(d => d.room == targetRoom);
    }
    public PlayerPlacedCheck FindRoom(int targetRoomIndex)
    {
        return roomList?.Find(d => d.room == (ROOM)targetRoomIndex);
    }
    public PlayerPlacedCheck FindRoom(string targetRoomName)
    {
        return roomList?.Find(d => d.room.ToString().Equals(targetRoomName));
    }
}
