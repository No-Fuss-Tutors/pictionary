using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public static bool loading;
    public static LoadingManager instance;

    private void Start()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            PhotonView newPhotonView = gameObject.AddComponent<PhotonView>();
            newPhotonView.ViewID = 990;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    //Forcefully closes the loading screen 
    public static void CloseLoadingMenu()
    {
        FindObjectOfType<LoadingMenu>().GetComponent<Animator>().Play("Close");
    }

    public static void OpenLoadingMenu()
    {
        FindObjectOfType<LoadingMenu>().GetComponent<Animator>().Play("Open");
    }

    [PunRPC]
    public void StartLoading()
    {
        StartButton startButton = FindObjectOfType<StartButton>();
        if(startButton != null)
        {
            startButton.startingGame = true;
            LoadingManager.OpenLoadingMenu();
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            StartCoroutine(LeaveRoom());
        }
    }

    public void ForceLeave()
    {
        StartCoroutine(LeaveRoom());
    }
        

    IEnumerator LeaveRoom()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {        
            OpenLoadingMenu();
            while(!loading)
            {
                yield return 0;
            }
            AsyncOperation loadingScene = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            while(!loadingScene.isDone)
            {
                yield return 0;
            }
            CloseLoadingMenu();
        }
    }
}
