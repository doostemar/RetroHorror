using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotFollower : MonoBehaviour
{
  GameObject m_Hero;
  BotChannel m_BotChannel;
  Vector3    m_OffsetTarget;

  void Start()
  {
    m_Hero = GameObject.FindGameObjectWithTag( "Hero" );
    HeroEventSystem hero_evts = m_Hero.GetComponent<HeroEventSystem>();
    hero_evts.OnHeroEvent += OnHeroEvent;

    m_BotChannel   = GetComponent<BotChannel>();
    m_OffsetTarget = transform.position - m_Hero.transform.position;
  }

  void OnHeroEvent( HeroEvent evt )
  {
    if ( evt.m_Type == HeroEvent.EventType.HeroStateMoving )
    {
      m_OffsetTarget = transform.position - m_Hero.transform.position;
    }
  }

  void Update()
  {
    BotMoveEvent move_evt     = ScriptableObject.CreateInstance<BotMoveEvent>();
    move_evt.m_Type           = BotMoveEvent.Type.Move;
    move_evt.m_TargetPosition = m_Hero.transform.position + m_OffsetTarget;

    Debug.Log( "Offset: " + m_OffsetTarget + " Position: " + move_evt.m_TargetPosition );

    m_BotChannel.RaiseMoveEvent( move_evt );
  }
}
