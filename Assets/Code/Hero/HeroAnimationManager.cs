using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimationManager : MonoBehaviour
{
  private Animator HeroAnimator;

  // Start is called before the first frame update
  void Start()
  {
    HeroAnimator = GetComponent<Animator>();
    HeroEventSystem hero_events = GetComponent<HeroEventSystem>();
    hero_events.OnHeroEvent += OnHeroEvent;
  }

  void OnHeroEvent(HeroEvent hero_event)
  {
    switch ( hero_event.m_Type )
    {
      case HeroEvent.EventType.HeroStateCasting:
      {
        HeroAnimator.Play("Resurrect");
      }
      break;
      case HeroEvent.EventType.HeroStateMoving:
      { 
        HeroAnimator.Play("Move"); 
      }
      break;
      case HeroEvent.EventType.HeroStateIdle:
      { 
        HeroAnimator.Play("ShadowIdle");
      }
      break;
    }
  }
}
