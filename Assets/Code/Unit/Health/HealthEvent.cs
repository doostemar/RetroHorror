using UnityEngine;

public class HealthEvent : ScriptableObject
{
  public enum Type
  {
    Damage,
    Heal,
    Dead,
    InvincibleStart,
    InvincibleEnd
  }

  public Type  m_Type;
  public float m_Value;
}