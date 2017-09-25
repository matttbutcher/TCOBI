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
    private bool artifact;
    
    // UI Names
    [SerializeField]
    private TextMesh medkitName;
    [SerializeField]
    private TextMesh artifactName;

    // Audio
    public AudioSource audioSource;
    public AudioClip[] pickupSound;

    [SerializeField]
    private float value;

    public void Interaction(PlayerVitals playerVitals)
    {
        if (food)
        {
            PlayPickupSound(0);
            playerVitals.hungerSlider.value += value;
        }
        else if (water)
        {
            PlayPickupSound(1);
            playerVitals.thirstSlider.value += value;
        }
        else if (health)
        {
            PlayPickupSound(2);
            playerVitals.medkitCount += 1;
            medkitName.text = "Medkits Used: " + playerVitals.medkitCount;
            playerVitals.healthSlider.value += value;
        }
        else if (artifact)
        {
            PlayPickupSound(3);
            playerVitals.artifactCount += 1;
            artifactName.text = "Artifacts Collected: " + playerVitals.artifactCount + "/25";
        }
    }

    public void PlayPickupSound (int clipNumber)
    {
        audioSource.clip = pickupSound[clipNumber];
        audioSource.Play();
    }
}