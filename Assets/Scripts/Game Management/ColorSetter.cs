using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDrawColor()
    {
        DrawTexture.drawColor = image.color;
    }

    public void SetDrawMode(DrawTexture.DrawMode mode, DrawTexture drawTexture)
    {
        drawTexture.drawMode = mode;
    }

    public void SetPencilMode()
    {
        FindObjectOfType<DrawTexture>().drawMode = DrawTexture.DrawMode.Pencil;
    }

    public void SetEraserMode()
    {
        FindObjectOfType<DrawTexture>().drawMode = DrawTexture.DrawMode.Eraser;
    }
}
