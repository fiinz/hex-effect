using System;
using UnityEngine;


public abstract class UIBaseElement : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public void OnEnable()
    {
        if(_canvasGroup==null)
        gameObject.AddComponent<CanvasGroup>();
        
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}