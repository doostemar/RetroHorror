using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
  public float m_MaxSpeed    = 1f;
  public float m_Accel       = 1f;
  public float m_Friction    = 0.5f;

  private Animator      m_Anim;
  private Vector2       m_Movement;

  const string kIdleAnimLeftName       = "Stalin";
  const string kIdleAnimRightName      = "StalinRight";
  const string kIdleAngryAnimLeftName  = "StalinIdleAngry";
  const string kIdleAngryAnimRightName = "StalinIdleAngryRight";
  const string kCastLeftName           = "StalinCast";
  const string kCastRightName          = "StalinCastRight";

  // Start is called before the first frame update
  void Start()
  {
    m_Movement = Vector2.zero;
    m_Anim = GetComponent<Animator>();
    m_Anim.Play( "Stalin" );
  }

  // Update is called once per frame
  void Update()
  {
    float h_control = Input.GetAxis( "Horizontal" );
    float v_control = Input.GetAxis( "Vertical" );

    Vector2 control_dir = new Vector2( h_control, v_control ).normalized;
    m_Movement += control_dir * m_Accel * Time.deltaTime;
    if (m_Movement.sqrMagnitude > m_MaxSpeed * m_MaxSpeed)
    {
      m_Movement = m_Movement.normalized * m_MaxSpeed;
    }
    
    if (h_control == 0f && v_control == 0f)
    {
      bool x_positive = Mathf.Sign( m_Movement.x ) == 1f;
      bool y_positive = Mathf.Sign( m_Movement.y ) == 1f;

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
    }

    transform.position += new Vector3( m_Movement.x, m_Movement.y, 0f ) * Time.deltaTime;
  }
}
