using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public Menu[] menus;
    public int initialMenuIndex;
    // Start is called before the first frame update
    void Enabled()
    {
        if(LobbyManager.firstLoad)
        {
            menus[initialMenuIndex].Enable(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnableMenu(int menuIndex)
    {
        menus[menuIndex].Enable(this);
    }



    [System.Serializable]
    public class Menu
    {
        public GameObject panel;
        public void Disable()
        {
            panel.GetComponent<CanvasGroupManager>().Disable();
        }
        public void Enable(PanelManager panelManager)
        {
            Menu[] menuList = panelManager.menus;
            for(int menuIndex = 0; menuIndex < menuList.Length; menuIndex++)
            {
                if(menuList[menuIndex] != this)
                {
                    menuList[menuIndex].Disable();
                }
                
            }
            panel.GetComponent<CanvasGroupManager>().Enable();
        }
        public void Enable()
        {
            Enable(FindObjectOfType<PanelManager>());
        }
    }
}
