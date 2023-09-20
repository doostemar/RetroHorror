using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitEvent : ScriptableObject
{
  public enum Type
  {
    Attack,
    AttackFinished,
    Resurrect
  }

  public Type m_Type;
}
