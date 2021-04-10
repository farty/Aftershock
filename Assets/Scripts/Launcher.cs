using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
  
    [SerializeField] Transform playerListContent;    
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] int maxPlayers = 2;
    [SerializeField] TMP_InputField _playerNickName;
    [SerializeField] TMP_Text playerNickName;
    string nickname;
    
    void Awake()
    {
        Instance = this;

        string path = Application.persistentDataPath + "/player.nickname";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            nickname = formatter.Deserialize(stream) as string;
        }
        else
        {
            nickname = "Player " + Random.Range(0, 1000).ToString("0000");
        }
        
    }

    void Start()
    {
        Debug.Log("Connecting to MASTER");
        PhotonNetwork.ConnectUsingSettings();  
        playerNickName.text = nickname;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to MASTER");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("JoinedLobby");
        PhotonNetwork.NickName = nickname;
    }
    

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); 
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }
  
    /*public override void OnRoomListUpdate (List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)            
            continue;            
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    */
    
    
    

    public void FindhGame()
    {        
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        CreateRoom();
    }
    void CreateRoom()
    {
        PhotonNetwork.CreateRoom("village");
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu ("room");
       
        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
           Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }        
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) 
    {
         Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
         if(PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                StartGame();
            }            
        }
    }    
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
        MenuManager.Instance.OpenMenu("in game ui");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetPlayerNickName()
    {
        nickname = _playerNickName.text;
        playerNickName.text = nickname;
        PhotonNetwork.NickName = nickname;

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.nickname";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, nickname);
        stream.Close();
    }
    
}
