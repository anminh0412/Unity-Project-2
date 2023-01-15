using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject escPanel;
    public GameObject loadingPanel;
    public Text roomName;
    public Text roomState;
    public bool escaped = false;

    //RoomScrollView
    public RoomItemp roomItempPrefab;
    List<RoomItemp> roomItempList = new List<RoomItemp>();
    public Transform contentObject;

    //Update Room List
    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    //Player List
    public List<PlayerItem> playerItemList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    //Player Select Character
    public GameObject playerSelectionItem;
    //public Transform playerSelectionParent;

    //Play Game Event
    public GameObject playButton;
    public GameObject changeStateButton;

    //Checking player in room
    public bool inRoom = false;

    //Checking player in lobby
    //public bool inLobby = false;
    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }
    public void OnClickCreate()
    {
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 10, BroadcastPropsChangeToAll = true });
        }
    }
    public override void OnJoinedLobby()
    {
        playerSelectionItem.GetComponent<SelectCharacterController>().SetPlayerInfo(PhotonNetwork.LocalPlayer);
    }
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        inRoom = true;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }    
    }
    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItemp item in roomItempList)
        {
            Destroy(item.gameObject);
        }
        roomItempList.Clear();

        foreach(RoomInfo room in list)
        {
            RoomItemp newRoom = Instantiate(roomItempPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            if(room.IsOpen)
                newRoom.SetRoomState("Open");
            else if (!room.IsOpen)
                newRoom.SetRoomState("Close");
            newRoom.SetRoomPlayer(room.PlayerCount, room.MaxPlayers);
            roomItempList.Add(newRoom);
        }
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void OnClickManagerRoom()
    {
        if (PhotonNetwork.CurrentRoom.IsOpen) PhotonNetwork.CurrentRoom.IsOpen = false;
        else if (!PhotonNetwork.CurrentRoom.IsOpen) PhotonNetwork.CurrentRoom.IsOpen = true;
        Debug.Log("chay vo ham r");
    }
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        if (escPanel) escaped = false;
        inRoom = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }
        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            playButton.SetActive(true);
            changeStateButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
            changeStateButton.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escaped = (escaped == false) ? (true) : (false);
        }

        if (escaped) escPanel.SetActive(true); else escPanel.SetActive(false);

        if (inRoom)
        {
            if (PhotonNetwork.CurrentRoom.IsOpen) roomState.text = "Close Room";
            else if (!PhotonNetwork.CurrentRoom.IsOpen) roomState.text = "Open Room";
        }
    }
    
    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("LoadingScene");
    }
    public void BackToConnectScene()
    {
        if (inRoom == true)
        {
            PhotonNetwork.LeaveRoom();
        }
        else if (inRoom == false)
        {
            PhotonNetwork.Disconnect();
            OnDisconnected(DisconnectCause.None);
        }
    }

    public void ExitGameScene()
    {
        PhotonNetwork.Disconnect();
        OnDisconnected(DisconnectCause.ApplicationQuit);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.None) SceneManager.LoadScene("Connect");
        else if (cause == DisconnectCause.ApplicationQuit) { Application.Quit();}
    }
}

