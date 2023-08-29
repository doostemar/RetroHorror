using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelfEvent : ScriptableObject
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
