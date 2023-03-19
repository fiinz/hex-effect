using System;
using System.Collections;
using System.Collections.Generic;
using _hexEffect.Scripts;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{

    public event Action StartGameClicked;
    public event Action TryAgainClicked;
    
    
    
    [SerializeField] private UILevelElement _uiLevelElement;
    [SerializeField] private UISelectionElement _uiSelectionElement; 
    [SerializeField] private UITimerElement _uiTimerElement;
    [SerializeField] private UIStartMenuPanelElement _uiStartMenuPanelElement;
    [SerializeField] private UILostPanelElement _uiLostPanelElement;

//    [SerializeField] private UITimerElement _ui;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(int minLetters, int maxLetters, int level, int maxTime)
    {
        _uiSelectionElement.Initialize( maxLetters);
        _uiSelectionElement.Hide();
        _uiLevelElement.UpdateLevel(1);
        _uiLevelElement.Hide();
        _uiTimerElement.Initialize(maxTime);
        _uiTimerElement.Hide();
        _uiStartMenuPanelElement.Hide();
        _uiLostPanelElement.Hide();
        
    }

    public void ShowStartMenuPanel()
    {
        _uiStartMenuPanelElement.Show();
    }
    public void ShowLostPanel(int currentLevel)
    {
        _uiLostPanelElement.UpdateInfo(currentLevel);
        _uiLostPanelElement.Show();
    }
    
    public void ShowGameUI()
    {
        _uiSelectionElement.Show();
        _uiLevelElement.Show();
        _uiTimerElement.Show();
    
        
    }
    public void UI_StartGame()
    {
        StartGameClicked?.Invoke();
        
    }
    public void UI_TryAgain()
    {
        TryAgainClicked?.Invoke();
    }
    
    public void UI_SeeSolution()
    {
        TryAgainClicked?.Invoke();
    }
    public void UpdateSelection(List<HexModel> hexes, Color c)
    {
        var hexLetters = new List<Char>();
        foreach (var hex in hexes)
        {
            hexLetters.Add(hex.Char);
        }
        _uiSelectionElement.UpdateLetters(hexLetters,  c);

    }

    
    public void UpdateLevel(int level)
    {
        _uiLevelElement.UpdateLevel(level);
        
    }
    
    public void UpdateTimer(float value)
    {
        _uiTimerElement.UpdateTime(value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideStartMenu()
    {
        _uiStartMenuPanelElement.Hide();
    }

    public void HideGameUI()
    {
        _uiSelectionElement.Hide();
        _uiLevelElement.Hide();
        _uiTimerElement.Hide();

        
    }

    public void HideLostPanel()
    {
        _uiLostPanelElement.Hide();
    }
}
