using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombie : MonoBehaviour
{
  BotChannel m_Channel;

  void Start()
  {
    m_Channel = GetComponent<BotChannel>();
    BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
    move_evt.m_Type = BotMoveEvent.Type.Move;
    move_evt.m_TargetPosition = new Vector2( -20, 20 );
    m_Channel.RaiseMoveEvent( move_evt );
  }
}
