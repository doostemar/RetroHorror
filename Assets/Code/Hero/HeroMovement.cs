using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
  [Range(0.85f, 1f)]
  public float m_InputSmoothing = 0.2f;
  public float m_MaxSpeed       = 1f;
  public float m_Accel          = 1f;
  public float m_Friction       = 0.5f;
  public bool  m_DebugDisplay   = false;

  private Animator          m_Anim;
  private Vector2           m_ControlDir;
  private Vector2           m_Movement;
  private DebugHeroMovement m_DebugDisplayObj;
  private bool              m_PrevDebugDisp;

  const string kIdleAnimLeftName       = "Stalin";
  const string kIdleAnimRightName      = "StalinRight";
  const string kIdleAngryAnimLeftName  = "StalinIdleAngry";
  const string kIdleAngryAnimRightName = "StalinIdleAngryRight";
  const string kCastLeftName           = "StalinCast";
  const string kCastRightName          = "StalinCastRight";

  // Start is called before the first frame update
  void Start()
  {
    m_ControlDir = Vector2.zero;
    m_Movement = Vector2.zero;
    m_Anim = GetComponent<Animator>();
    m_Anim.Play( "Stalin" );

    m_PrevDebugDisp = false;
    if ( m_DebugDisplay )
    {
      ToggleDebug( true );
    }
  }

  // Update is called once per frame
  void Update()
  {
    float h_control = Input.GetAxis( "Horizontal" );
    float v_control = Input.GetAxis( "Vertical" );

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
