using UnityEngine;
using UnityEngine.Events;

public class BotUnitChannel : MonoBehaviour
{
  public UnityAction<BotUnitEvent> OnUnitEvent;

  public void RaiseUnitEvent( BotUnitEvent unit_event )
  {
    if ( OnUnitEvent != null )
    {
      OnUnitEvent.Invoke( unit_event );
    }
    else
    {
      Debug.LogWarning( "A BotUnitEvent was requested, but nobody picked it up." );
    }
  }
}
