using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameSettingsManager : MonoBehaviourPun
{
    public Slider roundsSlider;
    public TMP_Text roundsText;

    public Slider timeSlider;
    public TMP_Text timeText;
    float syncTimer = 2f;
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
            if(syncTimer > 0)
            {
                syncTimer -= Time.deltaTime;
            }
            else
            {
                syncTimer = 2f;
                photonView.RPC("SyncSettings", RpcTarget.Others, GameSettings.rounds, GameSettings.seconds);
            }
        }
        else
        {
            roundsSlider.value = GameSettings.rounds;
            roundsText.text = "Rounds: " + GameSettings.rounds;

            timeSlider.value = GameSettings.seconds;
            timeText.text = "Time Per Round: " + GameSettings.seconds + " Seconds";
        }
    }

    [PunRPC]
    public void SyncSettings(int roundsSetting, int secondsSetting)
    {
        GameSettings.rounds = roundsSetting;
        GameSettings.seconds = secondsSetting;
    }


}
