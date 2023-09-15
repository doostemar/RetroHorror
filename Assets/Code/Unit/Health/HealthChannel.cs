using UnityEngine;
using UnityEngine.Events;

public class HealthChannel : MonoBehaviour
{
  public UnityAction<HealthEvent> OnHealthEvent;

  public void RaiseHealthEvent(HealthEvent health_event)
  {
    if (OnHealthEvent != null)
    {
      OnHealthEvent.Invoke(health_event);
    }
    else
    {
      Debug.LogWarning("A HealthEvent was requested, but nobody picked it up.");
    }
  }
}
