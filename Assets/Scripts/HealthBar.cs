using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;
	public TextMeshProUGUI healthText;

	private float maxHealth;
	public void setMaxHealth(float health)
	{
		slider.maxValue = health;
		slider.value = health;
		fill.color = gradient.Evaluate(1f);

		maxHealth = (float)Math.Round(health,1);
		healthText.SetText(maxHealth + "/" + maxHealth);
	}

	public void setHealth(float health)
	{
		slider.value = (float)Math.Round(health,1);
		fill.color = gradient.Evaluate(slider.normalizedValue);
		healthText.SetText((float)Math.Round(health,1) + "/" + maxHealth);
	}
}