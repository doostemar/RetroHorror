using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotChannel : MonoBehaviour
{
  public UnityAction<BotMoveEvent> OnMoveEvent;

  public void RaiseMoveEvent( BotMoveEvent move_event )
  {
    if ( OnMoveEvent != null )
    {
      OnMoveEvent.Invoke( move_event );
    }
    else
    {
      Debug.LogWarning( "A BotMoveEvent was requested, but nobody picked it up." );
    }
  }
}
