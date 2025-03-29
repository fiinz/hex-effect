using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWordLength : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int value;
    [SerializeField] private TMP_Text valueTMP;
    [SerializeField] private Image image;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
    public void SetValue(int value)
    {
        valueTMP.text = value.ToString();
    }
}
