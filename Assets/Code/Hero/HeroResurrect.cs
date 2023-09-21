using UnityEngine;

public class HeroResurrect : MonoBehaviour
{
  public float m_CooldownTimeS = 1f;

  private HeroEventSystem          m_HeroEvents;
  private ResurrectionEventChannel m_ResEvents;

  private enum State
  {
    None,
    CoolingDown,
    DisplayingCasting,
    WantsCast
  }

  private float m_TimeSinceCast     = 0f;
  private State m_State;

  private void Start()
  {
    m_State = State.None;
    m_TimeSinceCast = m_CooldownTimeS;

    m_HeroEvents = GetComponent<HeroEventSystem>();
    ResurrectionSystem res_system = Game.GetGameController().GetComponent<ResurrectionSystem>();
    m_ResEvents = res_system.m_ResChannel;
  }

  void Update()
  {
    const string kButtonName = "Resurrect";

    bool pressed_button  = Input.GetButtonDown( kButtonName );
    bool released_button = Input.GetButtonUp( kButtonName );

    switch ( m_State )
    {
      case State.None:
      {
        if ( pressed_button )
        {
          m_State = State.DisplayingCasting;
          goto case State.DisplayingCasting;
        }
      }
      break;
      case State.DisplayingCasting:
      {
        ResurrectionEvent res_event = ScriptableObject.CreateInstance<ResurrectionEvent>();
        res_event.m_Type          = released_button ? ResurrectionEvent.Type.Cast : ResurrectionEvent.Type.Display;
        res_event.m_Position      = transform.position;
        res_event.m_TimeToDisplay = m_CooldownTimeS;
        m_ResEvents.RaiseEvent( res_event );

        if ( released_button )
        {
          HeroEvent cast_event = ScriptableObject.CreateInstance<HeroEvent>();
          cast_event.m_Type = HeroEvent.EventType.HeroStateCasting;

          m_HeroEvents.RaiseEvent(cast_event);
          
          m_TimeSinceCast = 0f;
          m_State         = State.CoolingDown;
        }
      }
      break;
      case State.WantsCast:
      {
        if ( released_button )
        {
          m_State = State.CoolingDown;
        }
        TickCooldown();
      }
      break;
      case State.CoolingDown:
      {
        if ( pressed_button )
        {
          m_State = State.WantsCast;
        }
        TickCooldown();
      }
      break;
    }
  }

  void TickCooldown()
  {
    m_TimeSinceCast += Time.deltaTime;
    if ( m_TimeSinceCast >= m_CooldownTimeS )
    {
      HeroEvent cast_done_event = ScriptableObject.CreateInstance<HeroEvent>();
      cast_done_event.m_Type = HeroEvent.EventType.HeroStateCastDone;

      m_HeroEvents.RaiseEvent( cast_done_event );
      if ( m_State == State.WantsCast )
      {
        m_State = State.DisplayingCasting;
      }
      else
      {
        m_State = State.None;
      }
    }
  }
}
