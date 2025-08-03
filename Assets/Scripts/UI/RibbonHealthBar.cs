using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ribbon
{
    public class RibbonHealthBar : MonoBehaviour
    {

        public Sprite[] YarnSprites; // Array of sprites for the yarn segments IN ORDER FROM SMALL TO BIG
        public Image YarnImage;
        public Image HealthBar;

        public TMP_Text ButtonCountText;

        public int MaxHealth = 5;
        public int CurrentHealth = 5;

        public Player player;


        public void Start()
        {
            player.OnHealthChanged.AddListener(SetHealth); // Subscribe to the player's health change event
            player.OnButtonAmountChanged.AddListener(SetButtonCount); // Subscribe to the player's health change event
            MaxHealth = player.Health; // Initialize MaxHealth from the player's health
            SetHealth(MaxHealth); // Set the initial health to MaxHealth
        }

        public void SetButtonCount(int amount)
        {
            ButtonCountText.text = amount.ToString(); // Update the button count text with the provided amount
        }

        public void SetHealth(int health)
        {
            if (health < 0 || health > MaxHealth)
            {
                return;
            }
            CurrentHealth = health; // Set the current health to the provided value
            UpdateBar(); // Update the health bar display
        }


        public void UpdateBar()
        {
            float ratio = (float)(CurrentHealth) / (float)MaxHealth; // Calculate the health ratio

            HealthBar.fillAmount = ratio; // Update the health bar fill amount

            if (CurrentHealth == MaxHealth)
            {
                YarnImage.sprite = YarnSprites[YarnSprites.Length - 1]; // Set to the largest sprite if at max health
            }
            else if (CurrentHealth > 2)
            {
                YarnImage.sprite = YarnSprites[YarnSprites.Length - 2]; // Set to the second largest sprite if health is above 2
            }
            else
            {
                YarnImage.sprite = YarnSprites[0]; // Set to the smallest sprite if health is 0
            }
        }
    }

}