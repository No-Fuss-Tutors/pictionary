using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PictManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        LoadingManager.CloseLoadingMenu();
        if(PhotonNetwork.IsMasterClient)
        {
            LoadingManager.instance.photonView.RPC("StartLoading", RpcTarget.Others);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
