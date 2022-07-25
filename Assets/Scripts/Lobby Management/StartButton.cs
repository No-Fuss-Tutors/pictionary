using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class StartButton : MonoBehaviourPunCallbacks
{   
    public bool startingGame = false;
    public Button button;
    public int ready;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            Room room = PhotonNetwork.CurrentRoom;
            button.interactable = (room.PlayerCount >= 1) && PhotonNetwork.IsMasterClient;
            if(startingGame && LoadingManager.loading)
            {
                PhotonNetwork.LoadLevel(1);
                startingGame = false;
            }
        }
        else
        {
            button.interactable = false;
        }
        
    }

    
    public void StartGame()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            startingGame = true;
            ready = 0;
            LoadingManager.OpenLoadingMenu();
        }
    }

    

}
