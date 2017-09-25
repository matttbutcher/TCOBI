using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemProperties : MonoBehaviour
{

    [Header("Your Consumables")]
    public string itemName;

    private float artifactCount = 0;

    [SerializeField]
    private bool food;
    [SerializeField]
    private bool water;
    [SerializeField]
    private bool health;
    [SerializeField]
    private bool artifact;
    [SerializeField]
    private TextMesh artifactName;
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
        else if (artifact)
        {
            playerVitals.artifactCount += 1;
            artifactName.text = "Artifacts Collected: " + playerVitals.artifactCount;
        }
    }
}