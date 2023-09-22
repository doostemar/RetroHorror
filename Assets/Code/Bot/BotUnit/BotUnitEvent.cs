using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitEvent : ScriptableObject
{
  public enum Type
  {
    Attack,
    AttackFinished,
    Resurrect,
    MovementDirect
  }

  public Type    m_Type;
  public Vector2 m_Position;
}
