using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGroupManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float targetAlpha;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 8f);
        if(Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.005f)
        {
            canvasGroup.alpha = targetAlpha;
        }
    }

    public void Disable()
    {
        targetAlpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void Enable()
    {
        targetAlpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}
