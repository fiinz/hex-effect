using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private UISelectionElement _uiSelectionElement; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ShowHexSelection()
    {
        _uiSelectionElement.gameObject.SetActive(true);
        
    }

    void HideHexSelection()
    {
        _uiSelectionElement.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
