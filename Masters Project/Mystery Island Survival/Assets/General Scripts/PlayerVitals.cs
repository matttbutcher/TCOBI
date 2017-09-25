using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;

public class PlayerVitals : MonoBehaviour {

    public Slider healthSlider;
    public int maxHealth;
    public int healthFallRate;

    public Slider thirstSlider;
    public int maxThirst;
    public int thirstFallRate;

    public Slider hungerSlider;
    public int maxHunger;
    public int hungerFallRate;

    public Slider staminaSlider;
    public int maxStamina;
    private int staminaFallRate;
    public int staminaFallMult;
    private int staminaRegainRate;
    public int staminaRegainMult;

    [Range(1, 365)] public float artifactCount = 0;

    private CharacterController charController;
    private OVRPlayerController playerController;

    public TitleMenu Quit;

    void Start()
    {
        init();

    }

    void init()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        thirstSlider.maxValue = maxThirst;
        thirstSlider.value = maxThirst;
        hungerSlider.maxValue = maxHunger;
        hungerSlider.value = maxHunger;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
        staminaFallRate = 1;
        staminaRegainRate = 1;
        charController = GetComponent<CharacterController>();
        playerController = GetComponent<OVRPlayerController>();

    }

    void Update()
    {
        // HEALTH CONTROLLER
        if ((hungerSlider.value <= 0) && (thirstSlider.value <= 0))
        {
            //Make health decrease very fast if both hunger and thirst low
            healthSlider.value -= Time.deltaTime / healthFallRate * 10;

        } else if (hungerSlider.value <= 0 || thirstSlider.value <= 0)
        {
            //Make health decrease fast if just one value is low
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;
        }

        if (healthSlider.value <= 0)
        {
            CharacterDeath();
        }

        // HUNGER CONTROLLER
        if (hungerSlider.value >= 0)
        {
            // Let hunger naturally and gradually fall
            hungerSlider.value -= Time.deltaTime / hungerFallRate;
        }

        else if (hungerSlider.value <= 0)
        {
            //If hunger hits 0, dont let value keep decreasing
            hungerSlider.value = 0;
        }

        else if (hungerSlider.value >= maxHunger)
        {
            //If hunger hits max, dont let value keep increasing
            hungerSlider.value = maxHunger;
        }

        // THIRST CONTROLLER
        if (thirstSlider.value >= 0)
        {
            // Let thirst naturally and gradually fall
            thirstSlider.value -= Time.deltaTime / thirstFallRate;
        }

        else if (thirstSlider.value <= 0)
        {
            //If thirst hits 0, dont let value keep decreasing
            thirstSlider.value = 0;
        }

        else if (thirstSlider.value >= maxThirst)
        {
            //If thirst hits max, dont let value keep increasing
            thirstSlider.value = maxThirst;
        }

        // STAMINA CONTROLLER
        // -- Slider -- //
        if (charController.velocity.magnitude > 0 && (Input.GetKey(KeyCode.LeftShift) || OVRInput.Get(OVRInput.RawButton.RHandTrigger))) // If the character is moving and pressing sprint key
        {
            staminaSlider.value -= Time.deltaTime / staminaFallRate * staminaFallMult; // Decrease stamina slider according to set values if the user presses sprint
        }
        else
        {
            staminaSlider.value += Time.deltaTime / staminaRegainRate * staminaRegainMult; // Else if the sprint key is not held, increase the stamina slider according to set values
        }

        // -- Movement -- //
        if (staminaSlider.value >= maxStamina)
        {
            staminaSlider.value = maxStamina; // Only set it to the max value the slider holds
        }
        else if (staminaSlider.value <= 0)
        {
            staminaSlider.value = 0;          // Only allow it to go as low as 0
            playerController.GotStamina = false; // Set the boolean in the player controller script to false so that the player can no longer sprint
        }
        else if (staminaSlider.value >= 0)
        {
            playerController.GotStamina = true;  // When the level rises again, set the boolean in the player controller script to true so that the player can sprint again
        }
    }

    void CharacterDeath()
    {
        Quit.LoadMainMenu();
    }
}
