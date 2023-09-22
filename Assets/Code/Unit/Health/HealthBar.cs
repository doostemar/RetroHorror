using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  [Serializable]
  public struct ColorInfo
  {
    [SerializeField]
    public float m_Threshold;

    [SerializeField]
    public Color m_Color;
  };

  public Color       m_FullColor;
  public ColorInfo[] m_ColorThresholds;

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

    fill.color = m_FullColor;
    foreach ( ColorInfo col in m_ColorThresholds )
    {
      if ( health <= col.m_Threshold * slider.maxValue )
      {
        fill.color = col.m_Color;
      }
    }
  }
}
