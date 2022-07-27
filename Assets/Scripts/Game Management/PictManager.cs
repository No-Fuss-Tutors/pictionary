using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PictManager : MonoBehaviourPun
{
    public static int playerID = 0;
    public int playerCount = 1;
    public int round = 1;
    public static int currentDrawer = 0;
    public static string currentWord;

    public GameObject[] drawerPanels;
    public GameObject[] guesserPanels;
    public TMPro.TMP_Text roundText;
    bool switchPanels = false;
    [HideInInspector]
    public int guessedWord = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerID = 0;
        LoadingManager.CloseLoadingMenu();
        if(PhotonNetwork.IsMasterClient)
        {
            LoadingManager.instance.photonView.RPC("StartLoading", RpcTarget.Others);
        }
        else
        {
            photonView.RPC("SendID", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId);
        }
        switchPanels = (playerID != currentDrawer);
    }

    // Update is called once per frame
    void Update()
    {
        if(switchPanels != (playerID == currentDrawer))
        {
            //Switch between the guesser panels and drawer panels
            switchPanels = !switchPanels;
            foreach(GameObject guesserPanel in guesserPanels)
            {
                guesserPanel.SetActive(!switchPanels);
            }
            foreach(GameObject drawerPanel in drawerPanels)
            {
                drawerPanel.SetActive(switchPanels);
            }      
        }
        roundText.text = "Round " + round.ToString() + "/" + GameSettings.rounds.ToString();
    }

    [PunRPC]
    public void SetID(string target, int newID)
    {
        if(PhotonNetwork.LocalPlayer.UserId == target)
        {
            playerID = newID;
        }
    }

    [PunRPC]
    public void SendID(string targetUID)
    {
        playerCount++;
        photonView.RPC("SetID", RpcTarget.Others, targetUID, playerCount);
    }
}
