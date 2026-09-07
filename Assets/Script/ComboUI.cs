using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private Slider comboSlider;
    [SerializeField] private Image comboFill;
    private List<Color> comboColors;

    private IScoreHandler scoreHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        comboColors = new List<Color>
        {
            Color.yellow,
            Color.orange,
            Color.red,
            Color.darkRed
        };
        comboFill.color = comboColors[0];
        comboSlider.value = 0;
    }

    public void Initialize(IScoreHandler handler)
    {
        scoreHandler = handler;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateComboSlider();
    }
    
    void UpdateComboSlider()
    {
    }
}
