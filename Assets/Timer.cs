﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public Text timerText;

    private float startTime;
    private bool finished = false;
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
        {
            return;
        }
        float t = Time.time - startTime;
        
        string minutes  = ((int) t/60).ToString();
        string seconds = (t % 60).ToString("f0");
        timerText.text = "TEMPS RESTANT : "+ minutes + "min " + seconds + "s";

    }

    public void finish()
    {
        finished = true;
        timerText.color = Color.yellow;
    }
}
