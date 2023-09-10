using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
  // Start is called before the first frame update
  public float      m_MaxHealth;
  public HealthBar  m_HealthBar;

  private float     m_CurrentHealth;

  void Start()
  {
    m_CurrentHealth = m_MaxHealth;
    m_HealthBar.SetMaxHealth(m_CurrentHealth);
  }

  void TakeDamage(float damage)
  {
    m_CurrentHealth -= damage;
    m_HealthBar.SetHealth(m_CurrentHealth);
  }

  // Test
  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Space))
    {
      TakeDamage(2);
    }
  }
}
