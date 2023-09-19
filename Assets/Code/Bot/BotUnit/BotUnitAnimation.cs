using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitAnimation : MonoBehaviour
{
  BotChannel     m_BotChannel;
  BotUnitChannel m_UnitChannel;
  Animator       m_Animator;
  bool           m_Attacking;

  void Start()
  {
    m_Animator = GetComponent<Animator>();
    m_BotChannel = GetComponent<BotChannel>();
    m_UnitChannel = GetComponent<BotUnitChannel>();

    m_BotChannel.OnMoveEvent += OnBotEvent;
    m_UnitChannel.OnUnitEvent += OnUnitEvent;

    m_Animator.Play( "Idle" );
    m_Attacking = false;
  }

  private void OnDestroy()
  {
    m_BotChannel.OnMoveEvent -= OnBotEvent;
    m_UnitChannel.OnUnitEvent -= OnUnitEvent;
  }

  void OnBotEvent( BotMoveEvent evt )
  {
    if ( evt.m_Type == BotMoveEvent.Type.Move 
      || evt.m_Type == BotMoveEvent.Type.Resume )
    {
      m_Animator.Play( "Move" );
    }
    else if ( m_Attacking == false &&
            ( evt.m_Type == BotMoveEvent.Type.Pause
           || evt.m_Type == BotMoveEvent.Type.Stop
           || evt.m_Type == BotMoveEvent.Type.Arrived ) )
    {
      m_Animator.Play( "Idle" );
    }
  }

  void OnUnitEvent( BotUnitEvent evt )
  {
    if ( evt.m_Type == BotUnitEvent.Type.Attack )
    {
      m_Animator.Play( "Attack" );
    }
  }
}
