using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    [SerializeField]private Image fill;

    public void setMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void setValue(float health)
    {
        slider.value = health;
    }

    public void setColor(Color color)
    {
        fill.color = color;
    }

    public float getValue()
    {
        return slider.value;
    }
}
