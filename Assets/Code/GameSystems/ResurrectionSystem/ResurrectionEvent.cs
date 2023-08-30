using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionEvent : ScriptableObject
{
  public enum Type
  {
    Display,
    Cast
  }

  public Vector2 m_Position;
  public float   m_Radius;
  public float   m_TimeToDisplay;
  public Type    m_Type;
}
