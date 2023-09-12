using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

  // Below this percentage, the health bar turns yellow 
  private static float MEDIUM_HEALTH_MODIFIER = 0.6f;

  // Below this percentage, the health bar turns red
  private static float LOW_HEALTH_MODIFIER = 0.3f;
  
  public Slider slider;

  public Image fill;

  public void SetMaxHealth(float max_health)
  {
    slider.maxValue = max_health;
    slider.value = max_health;
  }

  public void SetHealth(float health)
  {
    slider.value = health;

    if (health >= slider.maxValue * MEDIUM_HEALTH_MODIFIER) fill.color = Color.green;
    else if (health > slider.maxValue * LOW_HEALTH_MODIFIER) fill.color = Color.yellow;
    else fill.color = Color.red;

  }
}
