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
            if((PictManager.playerID == PictManager.currentDrawer || PictManager.finshed))
            {

            }
            else
            {
                SendMessage();
            }
        }
    }

    public void SendMessage()
    {
        NetworkAddTextBox();
        if(inputField.text.Trim().ToLower() == PictManager.currentWord.ToLower())
        {
            Debug.Log("Awarding points");
            FindObjectOfType<PictManager>().RewardPoints();
        }
        inputField.text = "";
        inputField.Select();
    }

    [PunRPC]
    public void AddTextBox(string message, string sender, bool finished, int senderID)
    {
        textBoxCount++;
        ChatMessage newMessage = Instantiate(messagePrefab).GetComponent<ChatMessage>();
        if(message.ToLower() == PictManager.currentWord.ToLower() && senderID != PictManager.currentDrawer)
        {
            newMessage.transform.parent = messageContainer.transform;
            newMessage.background.color = Color.green;
            newMessage.message.text = "<color=white>"+sender+" got it</color>";
        }
        else
        {
            newMessage.transform.parent = messageContainer.transform;
            newMessage.background.color = textBoxColors[textBoxCount%2];
            string nameColor = "grey";
            if(finished || PictManager.currentDrawer == senderID)
            {
                nameColor = "#daa520";
            }
            newMessage.message.text = "<b><color="+nameColor+">"+sender + "</color></b>: " + message;
        }
    }

    public void NetworkAddTextBox()
    {
        photonView.RPC("AddTextBox", RpcTarget.All, inputField.text.Trim(), PhotonNetwork.NickName, PictManager.finshed, PictManager.playerID);
    }

    public void ChatBoxSelected(bool value)
    {
        chatboxSelected = value;
    }
}
