using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> rooms = new List<RoomInfo>();
    public GameObject playerIconPrefab;
    public Transform playerIconContainer;
    public int createdRoomMenuIndex;
    public TMP_Text roomCodeText;
    public PanelManager panelManager;
    //First load of this menu (SET TO FALSE WHEN THE MINIGAME IS CLOSED)
    public static bool firstLoad = true;
    // Start is called before the first frame update
    void Start()
    {
        if(firstLoad)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion("us");
            firstLoad = false;
        }
        else
        {
            panelManager.EnableMenu(0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Generates a Photon room with a unique room code
    public void CreateRoom()
    {
        //Setup Room Options
        Photon.Realtime.RoomOptions options = new Photon.Realtime.RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = false;
        string roomName = GenerateSequence(6);
        PhotonNetwork.CreateRoom(roomName, options);
        roomCodeText.text = "Room Code: " + roomName;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRoom(TMP_InputField inputField)
    {
        JoinRoom(inputField.text);
    }

    //Join room using room code
    public void JoinRoom(string roomCode)
    {
        //search for exiting room with the code by looping through all the rooms
        PhotonNetwork.JoinRoom(roomCode);
    }

    //Random letter generation
    public string GenerateSequence(int length)
    {
        //arrays to exclude for each number
        HashSet<int>[] exclusionLayers = new HashSet<int>[] {new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>()};
        //check all rooms for used room codes;
        for(int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
        {
            RoomInfo room = rooms[roomIndex];
            string roomCode = room.Name;
            for(int codeIndex = 0; codeIndex < length; codeIndex++)
            {
                char codeChar = roomCode[codeIndex];
                exclusionLayers[codeIndex].Add((int)codeChar);
            }
        }
        //use ASCII to generate random letters
        int lowerRange = 65;
        int upperRange = 90;
        int[] randomNumbers = new int[length];
        for(int numberIndex = 0; numberIndex < length; numberIndex ++)
        {
            randomNumbers[numberIndex] = GenerationUtility.GenerateRandomInteger(lowerRange, upperRange+9, exclusionLayers[numberIndex]);
        }
        //generate code by converting the randomly generated integers to ASCII letters 
        string code = "";
        for(int charIndex = 0; charIndex < length; charIndex++)
        {
            //if the index is above the upper range then add it as a string of a number 
            int generatedNumber = randomNumbers[charIndex];
            if(generatedNumber > upperRange)
            {
                code += (generatedNumber-upperRange).ToString();
            }
            else
            {
                code += (char)generatedNumber;
            }
        }
        Debug.Log(code);
        return code;
    }

    public void RegenerateIcons(bool regenerateUser = true)
    {
        //Reset container of player icons by destroying all the children
        for(int childIndex = 0; childIndex < playerIconContainer.childCount; childIndex++)
        {
            Destroy(playerIconContainer.GetChild(childIndex).gameObject);
        }
        Room currentRoom = PhotonNetwork.CurrentRoom;
        Player[] playerList = new Player[currentRoom.Players.Count];
        currentRoom.Players.Values.CopyTo(playerList, 0);
        for(int playerIndex = 0; playerIndex < currentRoom.PlayerCount; playerIndex++)
        {
            Player player = playerList[playerIndex];
            if(!(player.UserId == PhotonNetwork.LocalPlayer.UserId && !regenerateUser))
            {
                GameObject playerIcon = Instantiate(playerIconPrefab, playerIconContainer);
                playerIcon.GetComponent<PlayerIcon>().playerText.text = player.NickName;
            }
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        panelManager.EnableMenu(1);
        base.OnCreateRoomFailed(returnCode, message);    
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        rooms = roomList;
    }

    public override void OnConnected()
    {
        base.OnConnected();
        panelManager.EnableMenu(0);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        panelManager.EnableMenu(createdRoomMenuIndex);
        
        //Get the list of players
        Room currentRoom = PhotonNetwork.CurrentRoom;
        Player[] playerList = new Player[currentRoom.Players.Count];
        currentRoom.Players.Values.CopyTo(playerList, 0);
        roomCodeText.text = "Room Code: " + currentRoom.Name;

        //Create the icon for all the players that are already in the lobby
        RegenerateIcons(false);
        //Create the local player's icon
        GameObject userIcon = Instantiate(playerIconPrefab, playerIconContainer);
        userIcon.GetComponent<PlayerIcon>().playerText.text = PhotonNetwork.NickName;
        panelManager.EnableMenu(1);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //remove the leaving player's associated icon
        RegenerateIcons(true);
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if(newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            //create a new icon for the player;
            GameObject playerIcon = Instantiate(playerIconPrefab, playerIconContainer);
            playerIcon.GetComponent<PlayerIcon>().playerText.text = newPlayer.NickName;
        }
    }


}
