using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameSettingsManager : MonoBehaviour
{
    public Slider roundsSlider;
    public TMP_Text roundsText;

    public Slider timeSlider;
    public TMP_Text timeText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameSettings.rounds = (int)roundsSlider.value;
            roundsText.text = "Rounds: " + GameSettings.rounds;

            GameSettings.seconds = (int)timeSlider.value;
            timeText.text = "Time Per Round: " + GameSettings.seconds + " Seconds";
        }
    }
}
