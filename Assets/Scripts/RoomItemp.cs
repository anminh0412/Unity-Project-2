using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemp : MonoBehaviour
{
    public Text roomName;
    public Text currentPlayer;
    public Text roomState;
    LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }
    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }
    public void SetRoomState(string _currentState)
    {
        roomState.text = _currentState;
    }
    public void SetRoomPlayer(int _currentPlayer, int _maxPlayer)
    {
        currentPlayer.text = _currentPlayer.ToString() + "/" + _maxPlayer.ToString();
    }
    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }

}
