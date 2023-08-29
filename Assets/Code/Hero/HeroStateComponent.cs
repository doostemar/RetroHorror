using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateComponent : MonoBehaviour
{
  HeroSelfEventSystem m_HeroEvents;

  // Start is called before the first frame update
  void Start()
  {
    m_HeroEvents = GetComponent<HeroSelfEventSystem>();
    m_HeroEvents.OnHeroSelfEvent += OnSelfEvent;
  }

  public void OnSelfEvent( HeroSelfEvent hero_event )
  {
    if (hero_event.m_Type == HeroSelfEvent.EventType.HeroStateCastDone)
    {
      HeroSelfEvent idle_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
      idle_event.m_Type = HeroSelfEvent.EventType.HeroStateIdle;

      m_HeroEvents.RaiseEvent( idle_event );
    }
  }
}
