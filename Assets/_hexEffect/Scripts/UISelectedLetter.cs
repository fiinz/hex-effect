using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UISelectedLetter : UIBaseElement
{
    [SerializeField] private TMP_Text letterTMP;
    [SerializeField] private Image background;

    // Start is called before the first frame update
    public void SetBackgroundColor(Color c)
    {
        background.color = c;
    }
    public void SetLetter(Char c, Color color)
    {
        letterTMP.text = c.ToString().ToUpper();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
