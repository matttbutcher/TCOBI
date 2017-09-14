using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemProperties : MonoBehaviour
{

    [Header("Your Consumables")]
    public string itemName;

    [SerializeField]
    private bool food;
    [SerializeField]
    private bool water;
    [SerializeField]
    private bool health;
    [SerializeField]
    private float value;

    public void Interaction(PlayerVitals playerVitals)
    {
        if (food)
        {
            playerVitals.hungerSlider.value += value;
        }
        else if (water)
        {
            playerVitals.thirstSlider.value += value;
        }
        else if (health)
        {
            playerVitals.healthSlider.value += value;
        }
    }
}