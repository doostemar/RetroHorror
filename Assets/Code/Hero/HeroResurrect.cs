using UnityEngine;

public class HeroResurrect : MonoBehaviour
{
  public float m_CastDoneTimeSeconds = 1f;

  private HeroSelfEventSystem m_HeroEvents;
  private float m_TimeSinceCast = 0f;

  private void Start()
  {
    m_HeroEvents = GetComponent<HeroSelfEventSystem>();
  }

  void Update()
  {
    if ( Input.GetButtonUp( "Resurrect" ) )
    {
      Debug.Log("Casting");
      m_TimeSinceCast = 0f;

      HeroSelfEvent cast_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
      cast_event.m_Type = HeroSelfEvent.EventType.HeroStateCasting;

      m_HeroEvents.RaiseEvent( cast_event );
    }

    if ( m_TimeSinceCast < m_CastDoneTimeSeconds )
    {
      m_TimeSinceCast += Time.deltaTime;
      if ( m_TimeSinceCast >= m_CastDoneTimeSeconds )
      {
        Debug.Log("Cast Done");
        HeroSelfEvent cast_done_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
        cast_done_event.m_Type = HeroSelfEvent.EventType.HeroStateCastDone;

        m_HeroEvents.RaiseEvent( cast_done_event );
      }
    }
  }
}
