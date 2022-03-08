using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    private float totalTime = 300;
    private Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value -= Time.deltaTime/totalTime;
    }
}
