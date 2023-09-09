using UnityEngine;

public class HeroEvent : ScriptableObject
{
  public enum EventType
  {
    HeroStateIdle,
    HeroStateCasting,
    HeroStateCastDone,
    HeroStateMoving
  }

  public EventType m_Type;
}
