using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotEnemyEvent : ScriptableObject
{
  public enum Type
  {
    Attack,
    AttackFinished
  };

  public Type m_Type;
}
