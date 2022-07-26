using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LineTools
{
    public static DrawTexture.WritePackage DrawLine(this Texture2D tex, Vector2 p1, Vector2 p2, Color col, int diameter, ref List<DrawTexture.Stroke> historyList)
    {
        DrawTexture.WritePackage writePackage = new DrawTexture.WritePackage(col);
        Vector2 interpolatedPos = p1;
        float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float interpolationTime = 0;
        
        while ((int)interpolatedPos.x != (int)p2.x || (int)interpolatedPos.y != (int)p2.y) 
        {
            interpolatedPos = Vector2.Lerp(p1, p2, interpolationTime);
            interpolationTime += frac;
            for(int x = -diameter/2; x < diameter/2; x++)
            {
                for(int y = -diameter/2; y < diameter/2; y++)
                {
                    Color currentColor = tex.GetPixel((int)interpolatedPos.x + x, (int)interpolatedPos.y + y);
                    historyList[historyList.Count - 1].pixels.Add(new DrawTexture.OverwritePixel(new Vector2((int)interpolatedPos.x + x, (int)interpolatedPos.y + y), col, currentColor));
                    tex.SetPixel((int)interpolatedPos.x + x, (int)interpolatedPos.y + y, col);
                    writePackage.xCoords.Add((int)interpolatedPos.x + x);
                    writePackage.yCoords.Add((int)interpolatedPos.y + y);                  
                }
            }
        }
        return writePackage;
    }

    public static DrawTexture.WritePackage DrawLine(this Texture2D tex, Vector2 p1, Vector2 p2, Color col, int diameter)
    {
        DrawTexture.WritePackage writePackage = new DrawTexture.WritePackage(col);
        Vector2 interpolatedPos = p1;
        float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float interpolationTime = 0;
        
        while ((int)interpolatedPos.x != (int)p2.x || (int)interpolatedPos.y != (int)p2.y) 
        {
            interpolatedPos = Vector2.Lerp(p1, p2, interpolationTime);
            interpolationTime += frac;
            for(int x = -diameter/2; x < diameter/2; x++)
            {
                for(int y = -diameter/2; y < diameter/2; y++)
                {
                    Color currentColor = tex.GetPixel((int)interpolatedPos.x + x, (int)interpolatedPos.y + y);
                    tex.SetPixel((int)interpolatedPos.x + x, (int)interpolatedPos.y + y, col);
                    writePackage.xCoords.Add((int)interpolatedPos.x + x);
                    writePackage.yCoords.Add((int)interpolatedPos.y + y);                  
                }
            }
        }
        return writePackage;
    }
}
