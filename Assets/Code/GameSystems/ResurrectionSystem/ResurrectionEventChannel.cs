using UnityEngine;
using UnityEngine.Events;

public class ResurrectionEventChannel : MonoBehaviour
{
  public UnityAction<ResurrectionEvent> OnResurrectionEvent;

  public void RaiseEvent( ResurrectionEvent res_event )
  { 
    if ( OnResurrectionEvent != null )
    {
      OnResurrectionEvent.Invoke( res_event );
    }
    else
    {
      Debug.LogWarning( "A ResurrectionEvent was requested but nobody picked it up." );
    }
  }
}
