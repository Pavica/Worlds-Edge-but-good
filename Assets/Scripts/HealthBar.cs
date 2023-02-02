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
	public void SetMaxHealth(float health)
	{
		slider.maxValue = health;
		slider.value = health;
		fill.color = gradient.Evaluate(1f);

		maxHealth = health;
		healthText.SetText(maxHealth + "/" + maxHealth);
	}

	public void SetHealth(float health)
	{
		slider.value = health;
		fill.color = gradient.Evaluate(slider.normalizedValue);

		healthText.SetText(health + "/" + maxHealth);

	}

}