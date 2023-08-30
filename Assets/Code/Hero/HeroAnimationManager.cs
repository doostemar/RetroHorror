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
    HeroSelfEventSystem hero_events = GetComponent<HeroSelfEventSystem>();
    hero_events.OnHeroSelfEvent += OnHeroEvent;
  }

  void OnHeroEvent(HeroSelfEvent hero_event)
  {
    Debug.Log(hero_event.m_Type);
    switch (hero_event.m_Type)
    {
      case HeroSelfEvent.EventType.HeroStateCasting:
      {
        HeroAnimator.Play("Resurrect");
      }
      break;
      case HeroSelfEvent.EventType.HeroStateMoving:
      { 
        HeroAnimator.Play("Move"); 
      }
      break;
      case HeroSelfEvent.EventType.HeroStateIdle:
      { 
        HeroAnimator.Play("Shadow");
      }
      break;
    }
  }
}
