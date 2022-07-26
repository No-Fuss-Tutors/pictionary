using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PictManager : MonoBehaviourPun
{
    public static int playerID = 0;
    public int playerCount = 1;
    public static int currentDrawer = 0;
    public static string currentWord;
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
