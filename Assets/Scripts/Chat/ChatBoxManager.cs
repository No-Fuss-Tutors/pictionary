using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class ChatBoxManager : MonoBehaviourPun
{
    public TMP_InputField inputField;
    public TMP_Text placeholderText;
    [HideInInspector]
    public int textBoxCount = 0;
    Color[] textBoxColors = new Color[] {new Color(0.8584906f, 0.8584906f, 0.8584906f, 1f), new Color(0.95f, 0.95f, 0.95f)};
    public GameObject messagePrefab;
    public GameObject messageContainer;
    bool chatboxSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool playerTurn = PictManager.playerID == PictManager.currentDrawer;
        //inputField.interactable = (!playerTurn);
        placeholderText.text = (playerTurn) ? "Chat" : "Enter Guess";
        if(chatboxSelected && Input.GetKeyDown(KeyCode.Return))
        {
            NetworkAddTextBox();
        }
    }

    [PunRPC]
    public void AddTextBox(string message, string sender)
    {
        textBoxCount++;
        ChatMessage newMessage = Instantiate(messagePrefab).GetComponent<ChatMessage>();
        newMessage.transform.parent = messageContainer.transform;
        newMessage.background.color = textBoxColors[textBoxCount%2];
        newMessage.message.text = "<color=grey>"+sender + "</color>: " + message;
    }

    public void NetworkAddTextBox()
    {
        photonView.RPC("AddTextBox", RpcTarget.AllBufferedViaServer, inputField.text, PhotonNetwork.NickName);
    }

    public void ChatBoxSelected(bool value)
    {
        chatboxSelected = value;
    }
}
