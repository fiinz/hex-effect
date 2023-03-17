using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimerElement : UIBaseElement
{
    [SerializeField] private Slider slider;

    private float maxTime;
    public void Initialize(float maxTime)
    {

        this.maxTime = maxTime;
        UpdateTime(this.maxTime);
    }

    public void UpdateTime(float remainingTime)
    {
        slider.value =(float) remainingTime / maxTime;

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
