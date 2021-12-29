using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SliderVal : MonoBehaviour
{
    TextMeshProUGUI speedText;
    void Start()
    {
        speedText = GetComponent<TextMeshProUGUI>();
    }

    public void SpeedUpdate(float value)
    {
        float speed = (float) Math.Round(value, 2);

        speedText.text = "x" + speed;

        Algorithm.speed = (speed>1)? speed * 100: speed / 5;
    }
}
