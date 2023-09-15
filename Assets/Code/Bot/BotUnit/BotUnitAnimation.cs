using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitAnimation : MonoBehaviour
{
  BotChannel     m_BotChannel;
  BotUnitChannel m_UnitChannel;
  Animator       m_Animator;

  void Start()
  {
    m_Animator = GetComponent<Animator>();
    m_BotChannel = GetComponent<BotChannel>();
    m_UnitChannel = GetComponent<BotUnitChannel>();

    m_BotChannel.OnMoveEvent += OnBotEvent;
    m_UnitChannel.OnUnitEvent += OnUnitEvent;
  }

  private void OnDestroy()
  {
    m_BotChannel.OnMoveEvent -= OnBotEvent;
    m_UnitChannel.OnUnitEvent -= OnUnitEvent;
  }

  void OnBotEvent( BotMoveEvent evt )
  {

  }

  void OnUnitEvent( BotUnitEvent evt )
  {
    if ( evt.m_Type == BotUnitEvent.Type.Attack )
    {
      m_Animator.Play( "Attack" );
    }
  }
}
