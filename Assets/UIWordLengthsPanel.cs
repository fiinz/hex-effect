using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIWordLengthsPanel : UIBaseElement
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private UIWordLength _uiWordLengthPrefab;
    [SerializeField] private List<UIWordLength> _uiWordLengths;

    public List<string> words;
    private int _poolSize;
    public void Initialize(int poolSize)
    {
        _poolSize = poolSize;
        _uiWordLengths = new List<UIWordLength>();
        
        for (int i = 0; i < poolSize; i++) //pool the double
        {
            var newWordLength = Instantiate(_uiWordLengthPrefab, transform, false);
            _uiWordLengths.Add(newWordLength);
            newWordLength.gameObject.SetActive(false);
            
            
        }
        
    }

    public void SetWords(List<string> words)
    {
        this.words = words;
    }

    public void Reset()
    {
        for (int i = 0; i < _uiWordLengths.Count; i++)
        {
            _uiWordLengths[i].gameObject.SetActive(false);
            if (i <  words.Count)
            {
                _uiWordLengths[i].gameObject.SetActive(true);
                _uiWordLengths[i].SetValue(words[i].Length);
                _uiWordLengths[i].SetColor(defaultColor);
            }
        }
        
    }
    
    public void UpdateLengths(string word, Color c)
    {
        for (int i = 0; i < words.Count(); i++)
        {
            if (words[i]==word)
            {
                _uiWordLengths[i].SetColor(c);
            }
        }
    }

    public void HideAllLengths()
    {
        
    }
    

   

    private void Update()
    {

    }

 
    
}
