using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class DrawTexture : MonoBehaviourPun
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
    public List<Stroke> networkHistory = new List<Stroke>();
    public static int currentStroke = -1;
    public static int networkStroke = -1;

    int[] netXCoords;
    int[] netYCoords;
    float[] netR;
    float[] netG;
    float[] netB;

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
        texture.filterMode = FilterMode.Bilinear;
        texture.Apply(true);
         
        textureDestination.texture = texture;
        textureDestination.rectTransform.GetWorldCorners(vCorners);
        
    }

    // Update is called once per frame
    void Update()
    {
        textureDestination.color = new Color(1, 1, 1, 1);
        if(drawColor.a < 1)
        {
            drawColor = new Color(drawColor.r, drawColor.g, drawColor.b, 1);
        }
        if(photonView.IsMine && PictManager.currentDrawer == PictManager.playerID)
        {   
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            float xFactor = (float)Screen.currentResolution.width/(float)Screen.width;
            float yFactor = (float)Screen.currentResolution.height/(float)Screen.height;
            Debug.Log(xFactor.ToString() + ", " + yFactor.ToString());
            Debug.Log(Screen.width + ", " + Screen.height + " -> " + Screen.currentResolution.width + ", " + Screen.currentResolution.height);
            if(mouseInRange)
            {
                //Calculate Position
                Vector2 anchorPos = vCorners[0];
                Vector2 mappedPos = new Vector2(mousePos.x, mousePos.y) + new Vector2(-anchorPos.x, anchorPos.y);
                mappedPos = new Vector2(mappedPos.x*xFactor, mappedPos.y*yFactor);
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
                                    photonView.RPC("CleanHistory",RpcTarget.Others);
                                }
                            }
                            history.Add(new Stroke());
                            photonView.RPC("AddHistory", RpcTarget.Others);
                            currentStroke++;
                        }
                        if(lastMappedPos == null || resetMappedPosition)
                        {
                            resetMappedPosition = false;
                            lastMappedPos = mappedPos;
                        }
                        if(Input.GetMouseButton(0))
                        {
                            WritePackage pencilPackage = LineTools.DrawLine(texture, mappedPos, lastMappedPos, drawColor, brushSize, ref history);
                            texture.Apply(true);
                            //photonView.RPC("WritePixels", RpcTarget.OthersBuffered, pencilPackage.xCoords.ToArray(), pencilPackage.yCoords.ToArray(), pencilPackage.rgb.r, pencilPackage.rgb.g, pencilPackage.rgb.b);
                            photonView.RPC("WriteLine", RpcTarget.OthersBuffered, Mathf.RoundToInt(mappedPos.x), Mathf.RoundToInt(mappedPos.y), Mathf.RoundToInt(lastMappedPos.x), Mathf.RoundToInt(lastMappedPos.y), brushSize, pencilPackage.rgb.r, pencilPackage.rgb.g, pencilPackage.rgb.b);
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
                            photonView.RPC("AddHistory", RpcTarget.Others);
                            currentStroke++;
                        }
                        if(lastMappedPos == null || resetMappedPosition)
                        {
                            resetMappedPosition = false;
                            lastMappedPos = mappedPos;
                        }
                        if(Input.GetMouseButton(0))
                        {
                            WritePackage erasePackage = LineTools.DrawLine(texture, mappedPos, lastMappedPos, Color.white, eraserSize+6, ref history);
                            texture.Apply(true);
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

        public void Undo(Texture2D tex, bool changeStroke = true, bool networkUndo = true)
        {
            pixels.Reverse();
            if(networkUndo)
            {
                FindObjectOfType<DrawTexture>().photonView.RPC("NetworkUndo", RpcTarget.Others);
            }
            for(int pixelIndex = 0; pixelIndex < pixels.Count; pixelIndex++)
            {
                OverwritePixel change = pixels[pixelIndex];
                tex.SetPixel(change.pixelPosition.x, change.pixelPosition.y, change.oldColor);               
            }      
            tex.Apply(false);
            if(changeStroke)
            {
                currentStroke--;
            } 
            pixels.Reverse();
        }

        public void Redo(Texture2D tex, bool changeStroke = true, bool networkRedo = true)
        {
            if(networkRedo)
            {
                FindObjectOfType<DrawTexture>().photonView.RPC("NetworkRedo", RpcTarget.Others);
            }
            for(int pixelIndex = 0; pixelIndex < pixels.Count; pixelIndex++)
            {
                OverwritePixel change = pixels[pixelIndex];
                tex.SetPixel(change.pixelPosition.x, change.pixelPosition.y, change.newColor);
            }
            tex.Apply(false);
            if(changeStroke)
            {      
                currentStroke++;
            }
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

    [PunRPC]
    public void WritePixels(int[] xCoords, int[] yCoords, float r, float g, float b)
    {
        for(int posIndex = 0; posIndex < xCoords.Length; posIndex++)
        {
            texture.SetPixel(xCoords[posIndex], yCoords[posIndex], new Color(r, g, b));
        }
        texture.Apply();
    }

    [PunRPC]
    public void WriteBulk(int[] xCoords, int[] yCoords, float[] r, float[] g, float[] b)
    {
        for(int posIndex = 0; posIndex < xCoords.Length; posIndex++)
        {
            texture.SetPixel(xCoords[posIndex], yCoords[posIndex], new Color(r[posIndex], g[posIndex], b[posIndex]));
        }
        texture.Apply();
    }

    [PunRPC]
    public void AddHistory()
    {
        networkHistory.Add(new Stroke());
        networkStroke++;
    }


    [PunRPC]
    public void WriteLine(int x1, int y1, int x2, int y2, int diameter, float r, float g, float b)
    {
        LineTools.DrawLine(texture, new Vector2(x1, y1), new Vector2(x2, y2), new Color(r, g, b, 1), diameter, ref networkHistory);
        texture.Apply();
    }

    [PunRPC]
    public void NetworkUndo()
    {

        Debug.Log("Removing at index #" + networkStroke);
        networkHistory[networkStroke].Undo(texture, false, false);
        networkStroke--;
        
    }

    [PunRPC]
    public void NetworkRedo()
    {

        Debug.Log("Removing at index #" + networkStroke);
        if(networkStroke < networkHistory.Count)
        {
            networkHistory[networkStroke + 1].Redo(texture, false, false);
            networkStroke++;
        }
        
    }

    [PunRPC]
    public void CleanHistory()
    {
        networkHistory.RemoveAt(networkHistory.Count - 1);
    }

    public class WritePackage
    {
        public List<int> xCoords = new List<int>();
        public List<int> yCoords = new List<int>();
        public Color rgb;

        public WritePackage(Color initColor)
        {
            rgb = initColor;
        }
    }


    
}
