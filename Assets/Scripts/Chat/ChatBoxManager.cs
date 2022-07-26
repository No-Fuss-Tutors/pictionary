using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChatBoxManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text placeholderText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool playerTurn = PictManager.playerID == PictManager.currentDrawer;
        inputField.interactable = (!playerTurn);
        placeholderText.text = (playerTurn) ? "Chat" : "Enter Guess";
    }
}
