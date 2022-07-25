using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawTexture : MonoBehaviour
{
    // Start is called before the first frame update
    public static Color drawColor;
    [HideInInspector]
    public Texture2D texture;
    public RawImage textureDestination;
    public bool mouseInRange;
    BoxCollider2D collider;
    bool resetMappedPosition = false;
    public int brushSize;
    public Vector3[] vCorners = new Vector3[4];
    Vector2 lastMousepos;
    Vector2 lastMappedPos;

    void Start()
    {  
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
            if(lastMappedPos == null || resetMappedPosition)
            {
                resetMappedPosition = false;
                lastMappedPos = mappedPos;
            }
            if(Input.GetMouseButton(0))
            {
                LineTools.DrawLine(texture, mappedPos, lastMappedPos, drawColor, brushSize);
                texture.Apply(false, false);
            }
            
            lastMappedPos = mappedPos;
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

    
}
