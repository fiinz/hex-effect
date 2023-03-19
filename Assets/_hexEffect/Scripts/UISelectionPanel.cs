using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectionPanel : UIBaseElement
{
    // Start is called before the first frame update
    [SerializeField] private UISelectedLetter _uiSelectedLetterPrefab;
    private List<UISelectedLetter> _uiSelectedLetters;
    public int poolSize;
    public void Initialize(int poolSize)
    {
        this.poolSize = poolSize;
            
        _uiSelectedLetters = new List<UISelectedLetter>();
        
        for (int i = 0; i < poolSize; i++) //pool the double
        {
            var newLetter = Instantiate(_uiSelectedLetterPrefab, transform, false);
            _uiSelectedLetters.Add(newLetter);
            newLetter.gameObject.SetActive(false);
        }

    } 

    // Update is called once per frame
    public void UpdateLetters(List<Char> chars, Color c)
    {
     for (int i = 0; i < poolSize; i++)
        {
            _uiSelectedLetters[i].gameObject.SetActive(false);
            if (i < chars.Count)
            {

                _uiSelectedLetters[i].SetBackgroundColor(c);
                _uiSelectedLetters[i].SetLetter(chars[i],Color.black);
                _uiSelectedLetters[i].gameObject.SetActive(true);

            }

        }
    }
}
