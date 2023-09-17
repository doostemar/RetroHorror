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

  public Vector3 m_Direction;
  public Type    m_Type;
  public float   m_Value;
}