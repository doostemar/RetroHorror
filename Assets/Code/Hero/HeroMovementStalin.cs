using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroMovementStalin : MonoBehaviour
{
  [Range(0.85f, 1f)]
  public float m_InputSmoothing = 0.2f;
  public float m_MaxSpeed       = 1f;
  public float m_Accel          = 1f;
  public float m_Friction       = 0.5f;
  public bool  m_DebugDisplay   = false;

  private HeroEventSystem m_HeroEvents;

  private Vector2             m_ControlDir;
  private Vector2             m_Movement;
  private DebugHeroMovement   m_DebugDisplayObj;
  private bool                m_PrevDebugDisp;

  // Start is called before the first frame update
  void Start()
  {
    m_HeroEvents = GetComponent<HeroEventSystem>();
    m_HeroEvents.OnHeroEvent += OnSelfEvent;

    m_ControlDir = Vector2.zero;
    m_Movement   = Vector2.zero;

    m_PrevDebugDisp = false;
    if ( m_DebugDisplay )
    {
      ToggleDebug( true );
    }
  }

  public void OnSelfEvent( HeroEvent self_event )
  {
    if ( self_event.m_Type == HeroEvent.EventType.HeroStateCasting )
    {
      m_ControlDir = Vector2.zero;
      m_Movement   = Vector2.zero;
      enabled = false;
    }
    else
    {
      enabled = true;
    }
  }

  // Update is called once per frame
  void Update()
  {
    float h_control = Input.GetAxis( "Horizontal" );
    float v_control = Input.GetAxis( "Vertical" );

    bool was_moving = m_Movement != Vector2.zero;

    {
      // This is a linear low-pass filter. It helps to smooth out the input for us
      Vector2 new_control_dir = new Vector2(h_control, v_control).normalized;
      m_ControlDir = new_control_dir * (1f - m_InputSmoothing) + m_ControlDir * m_InputSmoothing;
    }

    m_Movement += m_ControlDir * m_Accel * Time.deltaTime;
    if (m_Movement.sqrMagnitude > m_MaxSpeed * m_MaxSpeed)
    {
      m_Movement = m_Movement.normalized * m_MaxSpeed;
    }
    
    bool y_positive = Mathf.Sign( m_Movement.y ) == 1f;
    bool x_positive = Mathf.Sign( m_Movement.x ) == 1f;

    m_Movement -= m_Movement.normalized * m_Friction * Time.deltaTime;

    if ( x_positive )
    {
      m_Movement.x = Mathf.Max( m_Movement.x, 0f );
    }
    else
    {
      m_Movement.x = Mathf.Min( m_Movement.x, 0f );
    }

    if ( y_positive )
    {
      m_Movement.y = Mathf.Max( m_Movement.y, 0f );
    }
    else
    {
      m_Movement.y = Mathf.Min( m_Movement.y, 0f );
    }

    bool is_moving = m_Movement != Vector2.zero;

    // Handle state stuff
    if ( was_moving == false && is_moving )
    {
      HeroEvent hero_event = ScriptableObject.CreateInstance<HeroEvent>();
      hero_event.m_Type = HeroEvent.EventType.HeroStateMoving;
      m_HeroEvents.RaiseEvent( hero_event );
    }

    if ( was_moving && is_moving == false )
    {
      HeroEvent hero_event = ScriptableObject.CreateInstance<HeroEvent>();
      hero_event.m_Type = HeroEvent.EventType.HeroStateIdle;
      m_HeroEvents.RaiseEvent( hero_event );
    }


    transform.position += new Vector3( m_Movement.x, m_Movement.y, 0f ) * Time.deltaTime;

    if ( m_DebugDisplay != m_PrevDebugDisp )
    {
      ToggleDebug( m_DebugDisplay );
    }

    if ( m_DebugDisplayObj )
    {
      m_DebugDisplayObj.m_MovementProperties.m_ControlDir = m_ControlDir;
      m_DebugDisplayObj.m_MovementProperties.m_Movement   = m_Movement;
      m_DebugDisplayObj.m_MovementProperties.m_MaxSpeed   = m_MaxSpeed;
    }
  }

  private void ToggleDebug( bool enable )
  {
    m_PrevDebugDisp = enable;
    if ( enable )
    { 
      if ( m_DebugDisplayObj == null )
      {
        m_DebugDisplayObj = gameObject.AddComponent<DebugHeroMovement>();
      }
      m_DebugDisplayObj.enabled = true;
    }
    else
    {
      if (m_DebugDisplayObj != null)
      {
        m_DebugDisplayObj.enabled = false;
      }
    }
  }
}
