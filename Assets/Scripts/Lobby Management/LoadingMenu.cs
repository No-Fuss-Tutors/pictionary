using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LoadingMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetLoading(bool loadingState)
    {
        LoadingManager.loading = loadingState;
    }

    public void SetLoadingTrue()
    {
        SetLoading(true);
    }

    public void SetLoadingFalse()
    {
        SetLoading(false);
    }
}
