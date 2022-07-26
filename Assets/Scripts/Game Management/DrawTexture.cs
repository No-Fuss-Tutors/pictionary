using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawTexture : MonoBehaviour
{
    // Start is called before the first frame update
    public DrawMode drawMode = DrawMode.Pencil;
    public static Color drawColor;
    [HideInInspector]
    public Texture2D texture;
    public RawImage textureDestination;
    public bool mouseInRange;
    BoxCollider2D collider;
    bool resetMappedPosition = false;
    public int brushSize;
    public int eraserSize;
    public Vector3[] vCorners = new Vector3[4];
    Vector2 lastMousepos;
    Vector2 lastMappedPos;

    //[HideInInspector]
    public List<Stroke> history = new List<Stroke>();
    public static int currentStroke = -1;

    void Start()
    {  
        currentStroke = - 1;
        lastMousepos = Input.mousePosition;
        collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector3(textureDestination.rectTransform.rect.width, textureDestination.rectTransform.rect.height);
        Debug.Log(new Vector3(textureDestination.rectTransform.rect.width, textureDestination.rectTransform.rect.height));
        drawColor = Color.black;
        texture = new Texture2D(Mathf.RoundToInt(textureDestination.rectTransform.rect.width), Mathf.RoundToInt(textureDestination.rectTransform.rect.height));
        for(int x = 0; x < texture.width; x++)
        {
            for(int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();
        textureDestination.texture = texture;
        textureDestination.rectTransform.GetWorldCorners(vCorners);
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        if(mouseInRange)
        {
            //Calculate Position
            Vector2 anchorPos = vCorners[0];
            Vector2 mappedPos = new Vector2(mousePos.x, mousePos.y) + new Vector2(-anchorPos.x, anchorPos.y);
            Vector2 roundedPos = new Vector2(Mathf.RoundToInt(mappedPos.x), Mathf.RoundToInt(mappedPos.y));
            switch(drawMode)
            {
                case DrawMode.Pencil:
                    if(Input.GetMouseButtonDown(0))
                    {
                        if(currentStroke < history.Count - 1)
                        {
                            int historySize = history.Count; 
                            while(currentStroke < historySize - 1)
                            {
                                history.RemoveAt(historySize - 1);
                                historySize--;
                            }
                        }
                        history.Add(new Stroke());
                        currentStroke++;
                    }
                    if(lastMappedPos == null || resetMappedPosition)
                    {
                        resetMappedPosition = false;
                        lastMappedPos = mappedPos;
                    }
                    if(Input.GetMouseButton(0))
                    {
                        LineTools.DrawLine(texture, mappedPos, lastMappedPos, drawColor, brushSize, ref history);
                        texture.Apply(false, false);
                    }
                    lastMappedPos = mappedPos;
                    break;
                case DrawMode.Eraser:
                    if(Input.GetMouseButtonDown(0))
                    {
                        if(currentStroke < history.Count - 1)
                        {
                            int historySize = history.Count; 
                            while(currentStroke < historySize - 1)
                            {
                                history.RemoveAt(historySize - 1);
                                historySize--;
                            }
                        }
                        history.Add(new Stroke());
                        currentStroke++;
                    }
                    if(lastMappedPos == null || resetMappedPosition)
                    {
                        resetMappedPosition = false;
                        lastMappedPos = mappedPos;
                    }
                    if(Input.GetMouseButton(0))
                    {
                        LineTools.DrawLine(texture, mappedPos, lastMappedPos, Color.white, eraserSize+6, ref history);
                        texture.Apply(false, false);
                    }
                    lastMappedPos = mappedPos;
                    break;
            }       
        }
        else
        {
            resetMappedPosition = true;
        }
        lastMousepos = mousePos;
    }

    public void PointerEnter()
    {
        mouseInRange = true;
    }

    public void PointerExit()
    {
        mouseInRange = false;
    }

    public static void SetColor(Color color)
    {
        drawColor = color;
    }

    public class Stroke
    {
        public List<OverwritePixel> pixels;

        public Stroke()
        {
            pixels = new List<OverwritePixel>();
        }

        public void Undo(Texture2D tex)
        {
            pixels.Reverse();
            for(int pixelIndex = 0; pixelIndex < pixels.Count; pixelIndex++)
            {
                OverwritePixel change = pixels[pixelIndex];
                tex.SetPixel(change.pixelPosition.x, change.pixelPosition.y, change.oldColor);
            }
            tex.Apply(false);
            currentStroke--;
            pixels.Reverse();
        }

        public void Redo(Texture2D tex)
        {
            Debug.Log("Low Leve; Redo!");
            for(int pixelIndex = 0; pixelIndex < pixels.Count; pixelIndex++)
            {
                OverwritePixel change = pixels[pixelIndex];
                tex.SetPixel(change.pixelPosition.x, change.pixelPosition.y, change.newColor);
            }
            tex.Apply(false);
            currentStroke++;
        }
    }

    public class OverwritePixel
    {
        public Vector2Int pixelPosition;
        public Color newColor;
        public Color oldColor;

        public OverwritePixel(Vector2 pixelPos, Color newCol, Color oldCol)
        {
            pixelPosition = new Vector2Int(Mathf.RoundToInt(pixelPos.x), Mathf.RoundToInt(pixelPos.y));
            newColor = newCol;
            oldColor = oldCol;
        }
    }

    public void SetBrushSize(int newSize)
    {
        brushSize = newSize;
    }

    public void SetBrushSize(Slider slider)
    {
        brushSize = Mathf.RoundToInt(slider.value);
    }

    
    public void SetEraserSize(int newSize)
    {
        eraserSize = newSize;
    }

    public void SetEraserSize(Slider slider)
    {
        eraserSize = Mathf.RoundToInt(slider.value);
    }

    public enum DrawMode
    {
        Pencil,
        Eraser
    }

    
}
