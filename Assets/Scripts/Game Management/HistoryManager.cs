using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    public DrawTexture drawTexture;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Undo()
    {
        Debug.Log("Undo!");
        if(DrawTexture.currentStroke > -1)
        {
            Debug.Log("Starting Undo!");
            drawTexture.history[DrawTexture.currentStroke].Undo(drawTexture.texture);
        }  
    }

    public void Redo()
    {
        Debug.Log("Redo!");
        if(DrawTexture.currentStroke + 1 < drawTexture.history.Count)
        {
            Debug.Log("Starting Redo!");
            drawTexture.history[DrawTexture.currentStroke + 1].Redo(drawTexture.texture);
        }
        
    }
}
