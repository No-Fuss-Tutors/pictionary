using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class NicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public Button createButton;
    public Button joinButton; 
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.NickName.Length > 0)
        {
            nicknameField.text = PhotonNetwork.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool validNickname = (PhotonNetwork.NickName.Length > 0);
        createButton.interactable = validNickname;
        joinButton.interactable = validNickname;
    }

    public void SetNickname()
    {
        PhotonNetwork.NickName = nicknameField.text;
    }
}
