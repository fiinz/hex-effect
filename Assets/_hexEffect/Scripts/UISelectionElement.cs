using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectionElement : UIBaseElement
{
    // Start is called before the first frame update
    [SerializeField] private UISelectedLetter _uiSelectedLetterPrefab;
    private List<UISelectedLetter> _uiSelectedLetters;
    public int maxLetters;
    public void Initialize(int maxLetters)
    {
        this.maxLetters = maxLetters;
        _uiSelectedLetters = new List<UISelectedLetter>();
        
        for (int i = 0; i < maxLetters*2; i++) //pool the double
        {
            var newLetter = Instantiate(_uiSelectedLetterPrefab, transform, false);
            _uiSelectedLetters.Add(newLetter);
            newLetter.gameObject.SetActive(false);
        }

    } 

    // Update is called once per frame
    public void UpdateLetters(List<Char> chars, Color c)
    {
     for (int i = 0; i < maxLetters; i++)
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
