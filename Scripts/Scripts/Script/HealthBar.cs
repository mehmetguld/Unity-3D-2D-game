using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar: MonoBehaviour
{
    public Slider slider;


    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

    }

    public void SetHealt(int health)
    {
        slider.value = health;

    }

    internal static void SetHealth(int currentHealth)
    {
        throw new NotImplementedException();
    }
}
