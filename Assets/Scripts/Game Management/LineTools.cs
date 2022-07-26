using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineTools
{
    public static void DrawLine(this Texture2D tex, Vector2 p1, Vector2 p2, Color col, int diameter, ref List<DrawTexture.Stroke> historyList)
    {
        Vector2 t = p1;
        float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float ctr = 0;
        
        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) 
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            for(int x = -diameter/2; x < diameter/2; x++)
            {
                for(int y = -diameter/2; y < diameter/2; y++)
                {
                    Color currentColor = tex.GetPixel((int)t.x + x, (int)t.y + y);
                    historyList[historyList.Count - 1].pixels.Add(new DrawTexture.OverwritePixel(new Vector2((int)t.x + x, (int)t.y + y), col, currentColor));
                    tex.SetPixel((int)t.x + x, (int)t.y + y, col);
                    
                }
            }
            
        }
    }
}
