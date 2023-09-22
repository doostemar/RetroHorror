using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitAnimation : MonoBehaviour
{
  BotChannel     m_BotChannel;
  BotUnitChannel m_UnitChannel;
  Animator       m_Animator;
  bool           m_Attacking;

  static int    kIdleStateId      = Animator.StringToHash( "Idle" );
  static int    kResurrectStateId = Animator.StringToHash( "Resurrection" );
  static int    kAttackStateId    = Animator.StringToHash( "Attack" );
  static int    kMoveStateId      = Animator.StringToHash( "Move" );
  static string kHealthBarName    = "Health Bar Canvas";

  void Start()
  {
    m_Animator = GetComponent<Animator>();
    m_BotChannel = GetComponent<BotChannel>();
    m_UnitChannel = GetComponent<BotUnitChannel>();

    m_BotChannel.OnMoveEvent += OnBotEvent;
    m_UnitChannel.OnUnitEvent += OnUnitEvent;

    m_Animator.Play( kIdleStateId );
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
      m_Animator.Play( kMoveStateId );
    }
    else if ( m_Attacking == false &&
            ( evt.m_Type == BotMoveEvent.Type.Pause
           || evt.m_Type == BotMoveEvent.Type.Stop
           || evt.m_Type == BotMoveEvent.Type.Arrived ) )
    {
      m_Animator.Play( kIdleStateId );
    }
    else if ( evt.m_Type == BotMoveEvent.Type.DirectionLeft )
    {
      Vector3 x_flip = new Vector3(-1, 1, 1);
      transform.localScale  = x_flip;

      Transform health_bar  = transform.Find( kHealthBarName );
      health_bar.localScale = x_flip;
    }
    else if ( evt.m_Type == BotMoveEvent.Type.DirectionRight )
    {
      transform.localScale = Vector3.one;

      Transform health_bar = transform.Find(kHealthBarName);
      health_bar.localScale = Vector3.one;
    }
  }

  void OnUnitEvent( BotUnitEvent evt )
  {
    if ( evt.m_Type == BotUnitEvent.Type.Attack )
    {
      m_Animator.Play( kAttackStateId );
    }
    else if ( evt.m_Type == BotUnitEvent.Type.Resurrect )
    {
      if ( m_Animator.HasState( 0, kResurrectStateId ) )
      {
        m_Animator.Play( kResurrectStateId );
      }
    }
  }
}
