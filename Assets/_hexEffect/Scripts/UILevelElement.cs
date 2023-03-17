
using TMPro;
using UnityEngine;




public class UILevelElement : UIBaseElement
{
    [SerializeField] private TMP_Text _level;


    public void UpdateLevel(int value)
    {
        _level.text = "LEVEL " + value;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
