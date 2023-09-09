using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateComponent : MonoBehaviour
{
  HeroEventSystem m_HeroEvents;

  // Start is called before the first frame update
  void Start()
  {
    m_HeroEvents = GetComponent<HeroEventSystem>();
    m_HeroEvents.OnHeroEvent += OnSelfEvent;
  }

  public void OnSelfEvent( HeroEvent hero_event )
  {
    if (hero_event.m_Type == HeroEvent.EventType.HeroStateCastDone)
    {
      HeroEvent idle_event = ScriptableObject.CreateInstance<HeroEvent>();
      idle_event.m_Type = HeroEvent.EventType.HeroStateIdle;

      m_HeroEvents.RaiseEvent( idle_event );
    }
  }
}
