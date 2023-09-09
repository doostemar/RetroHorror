using UnityEngine;
using UnityEngine.Events;

public class HeroEventSystem : MonoBehaviour
{
  public UnityAction<HeroEvent> OnHeroEvent;

  public void RaiseEvent( HeroEvent hero_event )
  {
    if ( OnHeroEvent != null)
    {
      OnHeroEvent.Invoke( hero_event );
    }
    else
    {
      Debug.LogWarning("A HeroSelfEvent was requested, but nobody picked it up.");
    }
  }
}
