using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BotEnemyChannel : MonoBehaviour
{
  public UnityAction<BotEnemyEvent> OnEnemyEvent;

  public void RaiseEnemyEvent( BotEnemyEvent enemy_event )
  {
    if ( OnEnemyEvent != null )
    {
      OnEnemyEvent.Invoke( enemy_event );
    }
    else
    {
      Debug.LogWarning( "A BotEnemyEvent was requested, but nobody picked it up." );
    }
  }
}
