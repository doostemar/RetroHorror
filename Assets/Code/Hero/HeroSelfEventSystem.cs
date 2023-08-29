using UnityEngine;
using UnityEngine.Events;

public class HeroSelfEventSystem : MonoBehaviour
{
  public UnityAction<HeroSelfEvent> OnHeroSelfEvent;

  public void RaiseEvent( HeroSelfEvent hero_event )
  {
    if ( OnHeroSelfEvent != null)
    {
      OnHeroSelfEvent.Invoke( hero_event );
    }
    else
    {
      Debug.LogWarning("A HeroSelfEvent was requested, but nobody picked it up.");
    }
  }
}
