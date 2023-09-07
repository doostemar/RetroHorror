using UnityEngine;

public class BotMoveEvent : ScriptableObject
{
  public enum Type
  {
    Move,
    Stop,
    Pause,
    Resume,
  };

  public Type    m_Type;
  public Vector2 m_TargetPosition;
}
