using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILostPanelElement : UIBaseElement
{
    [SerializeField] private TMP_Text report;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInfo(int currentLevel)
    {
        report.text = $"You have reached level {currentLevel}. Please try again.";
        
    }
}
