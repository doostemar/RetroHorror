using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroMovementShadow : MonoBehaviour
{
  public float m_Velocity = 2f;

  private HeroSelfEventSystem m_HeroEvents;
  private bool                m_PrevIsMoving;

  // Start is called before the first frame update
  void Start()
  {
    m_HeroEvents = GetComponent<HeroSelfEventSystem>();
    m_HeroEvents.OnHeroSelfEvent += OnSelfEvent;

    m_PrevIsMoving = false;
  }

  public void OnSelfEvent(HeroSelfEvent self_event)
  {
    if (self_event.m_Type == HeroSelfEvent.EventType.HeroStateCasting)
    {
      enabled = false;
    }
    else
    {
      enabled = true;
    }
  }

  void Update()
  {
    Vector3 player_input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

    bool is_moving = player_input != Vector3.zero;

    transform.position += player_input * m_Velocity * Time.deltaTime;

    // Handle state stuff
    if ( is_moving && !m_PrevIsMoving )
    {
      HeroSelfEvent hero_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
      hero_event.m_Type = HeroSelfEvent.EventType.HeroStateMoving;  
      m_HeroEvents.RaiseEvent(hero_event);
    }
    else if ( !is_moving && m_PrevIsMoving )
    {
      HeroSelfEvent hero_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
      hero_event.m_Type = HeroSelfEvent.EventType.HeroStateIdle;
      m_HeroEvents.RaiseEvent(hero_event);
    }

    m_PrevIsMoving = is_moving;
  }
}
