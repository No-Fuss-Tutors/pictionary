using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Details : MonoBehaviour
{
    public string nickName;
    public int points;
    public int playerID;
    public TMP_Text nameText;
    public TMP_Text pointsText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        nameText.text = nickName;
        pointsText.text = points.ToString() + " points";
    }
}
