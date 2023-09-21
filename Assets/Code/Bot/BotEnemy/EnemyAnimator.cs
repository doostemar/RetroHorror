using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
  BotChannel      m_BotChannel;
  BotEnemyChannel m_EnemyChannel;
  Animator        m_Animator;

  private void Start()
  {
    m_Animator     = GetComponent<Animator>();
    m_BotChannel   = GetComponent<BotChannel>();
    m_EnemyChannel = GetComponent<BotEnemyChannel>();

    m_BotChannel.OnMoveEvent    += OnBotEvent;
    m_EnemyChannel.OnEnemyEvent += OnEnemyEvent;
  }

  private void OnDestroy()
  {
    m_BotChannel.OnMoveEvent    -= OnBotEvent;
    m_EnemyChannel.OnEnemyEvent -= OnEnemyEvent;
  }

  void OnBotEvent( BotMoveEvent evt )
  {

  }

  void OnEnemyEvent( BotEnemyEvent evt )
  {
    if ( evt.m_Type == BotEnemyEvent.Type.Attack )
    {
      m_Animator.Play( "Attack" );
    }
    else if ( evt.m_Type == BotEnemyEvent.Type.Die )
    {
      m_Animator.Play( "Death" );
    }
  }
}
