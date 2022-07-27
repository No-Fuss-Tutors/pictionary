using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;

public class PictManager : MonoBehaviourPun
{
    public static int playerID = 0;
    public int playerCount = 1;
    public int round = 1;
    public float timeLeft = 0;
    public static int currentDrawer = -1;
    public static int difficulty = 0;
    public static string currentWord = "";

    public GameObject[] drawerPanels;
    public GameObject[] guesserPanels;
    public TMPro.TMP_Text roundText;
    public TMPro.TMP_Text wordDisplay;
    public TMPro.TMP_Text timeDisplay;
    public GameObject detailsPrefab;
    public GameObject detailsContainer;
    public CanvasGroup choiceGroup;
    public Details[] details;
    bool switchPanels = false;
    public static bool finshed = false;
    [HideInInspector]
    public int guessedWord = 0;
    public static int[] difficultyModifier = new int[] {1,2,3};
    [HideInInspector]
    public int points = 0;
    //playerID, points
    public Dictionary<int,int> pointsDict = new Dictionary<int, int>();
    //playerID, nickname
    public Dictionary<int, string> nameDict = new Dictionary<int, string>();
    public static bool waiting = false;
    public string[] wordChoices;
    Coroutine buttonChoice;

    // Start is called before the first frame update
    void Start()
    {
        waiting = false;
        playerID = 0;
        details = new Details[100];
        LoadingManager.CloseLoadingMenu();
        if(PhotonNetwork.IsMasterClient)
        {
            LoadingManager.instance.photonView.RPC("StartLoading", RpcTarget.Others);
            photonView.RPC("SyncSettings", RpcTarget.AllBuffered, GameSettings.rounds, GameSettings.seconds);
            AssignPoints(playerID, 0);
            nameDict.Add(playerID, PhotonNetwork.NickName);
            StartCoroutine(WaitForPlayers());
        }
        else
        {
            photonView.RPC("SendID", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.NickName);
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
        if(playerID == currentDrawer)
        {
            wordDisplay.text = currentWord;
        }
        else
        {
            wordDisplay.text = WordUtility.HiddenWord(currentWord, timeLeft, (float)GameSettings.seconds);
        }
        if(timeLeft > 0 && PhotonNetwork.IsMasterClient)
        {
            if(currentDrawer >= 0 && !waiting)
            {
                timeLeft -= Time.deltaTime;
            }
            photonView.RPC("SyncTime", RpcTarget.Others, timeLeft);
        }
        else if(PhotonNetwork.IsMasterClient && currentDrawer >= 0)
        {
            StartRound();
        }
        timeDisplay.text = Mathf.CeilToInt(timeLeft).ToString() + " s";
        
    }

    IEnumerator WaitForPlayers()
    {
        while(playerCount < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            yield return 0;
        }
        int[] pointKeys =  pointsDict.Keys.ToArray<int>();
        List<int> pointValues = new List<int>();
        List<string> nameValues = new List<string>();
        foreach(int key in pointKeys)
        {
            pointValues.Add(pointsDict[key]);
            nameValues.Add(nameDict[key]);
        }
        photonView.RPC("SetupGame", RpcTarget.All, pointKeys, pointValues.ToArray(), nameValues.ToArray());
        StartRound();
    }

    //Run by other clients
    [PunRPC]
    public void SetID(string target, int newID)
    {
        if(PhotonNetwork.LocalPlayer.UserId == target)
        {
            playerID = newID;
        }
    }

    [PunRPC]
    public void SyncTime(float newTime)
    {
        timeLeft = newTime;
    }

    public void AssignPoints(int pID, int newPoints)
    {
        if(pointsDict.ContainsKey(pID))
        {
           pointsDict[pID] = newPoints;
        }
        else
        {
            pointsDict.Add(pID, points);
        }
    }
    [PunRPC]
    public void NetworkAssignPoints(int pID, int newPoints)
    {
        if(pointsDict.ContainsKey(pID))
        {
           pointsDict[pID] = newPoints; 
           details[pID].points = newPoints;
        }
        else
        {
            pointsDict.Add(pID, points);
        }
    }

    //Run by masterclient
    [PunRPC]
    public void SendID(string targetUID, string targetNickname)
    {
        photonView.RPC("SetID", RpcTarget.Others, targetUID, playerCount);
        AssignPoints(playerCount, 0);
        nameDict.Add(playerCount, targetNickname);
        playerCount++;
        photonView.RPC("SyncSettings", RpcTarget.Others, GameSettings.rounds, GameSettings.seconds);
    }
    [PunRPC]
    public void SetupGame(int[] pIDs, int[] pointValues, string[] nameValues)
    {
        for(int idIndex = 0; idIndex < pIDs.Length; idIndex++)
        {
            AssignPoints(pIDs[idIndex], pointValues[idIndex]);
            if(!nameDict.ContainsKey(pIDs[idIndex]))
            {
                nameDict.Add(pIDs[idIndex], nameValues[idIndex]);
            }
            Details newDetails = Instantiate(detailsPrefab).GetComponent<Details>();
            newDetails.transform.parent = detailsContainer.transform;
            newDetails.nickName = nameValues[idIndex];
            newDetails.points = pointValues[idIndex];
            details[pIDs[idIndex]] = newDetails;
        }
        
    }

    public void StartRound()
    {
        difficulty = Random.Range(0, 3);
        string[] words = WordUtility.GenerateWords();
        photonView.RPC("SyncRound", RpcTarget.AllBuffered, words, difficulty);
    }

    [PunRPC]
    public void SyncSettings(int roundsSetting, int secondsSetting)
    {
        GameSettings.rounds = roundsSetting;
        GameSettings.seconds = secondsSetting;
    }

    IEnumerator WaitForChoice()
    {
        Debug.Log("Waiting for choices");
        waiting = true;
        Debug.Log("Details: " + playerID.ToString() + ", " + currentDrawer.ToString());
        if(currentDrawer >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            currentDrawer = 0;
            round++;
        }
        if(currentDrawer == playerID)
        {
            Debug.Log("Prompting");
            buttonChoice = StartCoroutine(EnableChoices());
        }
        while(currentWord == "")
        {
            yield return 0;
        }
        waiting = false;
        NewRound();
    }

    [PunRPC]
    public void SyncRound(string[] words, int diff)
    {
        wordChoices = words;
        currentWord = "";
        difficulty = diff;
        currentDrawer++;
        finshed = false;
        timeLeft = GameSettings.seconds;
        StartCoroutine(WaitForChoice());
    }

    [PunRPC]
    public void SyncWord(string newWord)
    {
        currentWord = newWord;
    }

    public void NewRound()
    {
        
        DrawTexture.ResetDrawTexture();
    }

    public void RewardPoints()
    {
        finshed = true;
        int pointsGained = Mathf.CeilToInt(40*difficultyModifier[difficulty] + (timeLeft/(float)GameSettings.seconds)*100);
        points += pointsGained;
        photonView.RPC("NetworkAssignPoints", RpcTarget.All, playerID, points);
        photonView.RPC("RewardDrawingPoints", RpcTarget.All, pointsGained);
    }
    [PunRPC]
    public void RewardDrawingPoints(int pointsGained)
    {
        if(playerID == currentDrawer)
        { 
            points += Mathf.CeilToInt(pointsGained/3f);
            photonView.RPC("NetworkAssignPoints", RpcTarget.All, playerID, points);
        }
    }

    public void ChooseWord(int wordIndex)
    {
        photonView.RPC("SyncWord", RpcTarget.All, wordChoices[wordIndex]);
        StopCoroutine(buttonChoice);
        buttonChoice = StartCoroutine(DisableChoices());
    }

    IEnumerator EnableChoices()
    {
        choiceGroup.blocksRaycasts = true;
        choiceGroup.interactable = true;
        WordChoices choiceText = choiceGroup.GetComponent<WordChoices>();
        choiceText.easyText.text = wordChoices[0];
        choiceText.mediumText.text = wordChoices[1];
        choiceText.hardText.text = wordChoices[2];
        while(choiceGroup.alpha < 0.99f)
        {
            choiceGroup.alpha = Mathf.Lerp(choiceGroup.alpha, 1f, Time.deltaTime*8f);
            yield return 0;
        }
        choiceGroup.alpha = 1f;
    }

    IEnumerator DisableChoices()
    {
        choiceGroup.blocksRaycasts = false;
        choiceGroup.interactable = false;
        while(choiceGroup.alpha > 0.01f)
        {
            choiceGroup.alpha = Mathf.Lerp(choiceGroup.alpha, 0f, Time.deltaTime*8f);
            yield return 0;
        }
        choiceGroup.alpha = 0f;
    }
}
