using System;
using UnityEngine;

public class DebugHeroMovement : MonoBehaviour
{
  DebugArrowRenderer m_ControlRenderer;
  DebugArrowRenderer m_MovementRenderer;
  DebugArrowRenderer m_MaxSpeedRenderer;
  DebugCircleRenderer m_ControlOutline;
  DebugCircleRenderer m_MovementOutline;

  // white
  Color m_Color = new Color( 1f,
                             1f, 
                             1f );
  // green
  Color m_SpeedColor = new Color( 51f  / 255f,
                                  140f / 255f,
                                  27f  / 255f );

  Vector2 m_ScreenPosition = new Vector2( 100f, 200f );
  float   m_ArrowSizePx    = 30f;

  public struct MovementProperties
  {
    public Vector2 m_ControlDir;
    public Vector2 m_Movement;
    public float   m_MaxSpeed;
  }

  public MovementProperties m_MovementProperties;

  void OnEnable()
  {
    m_MovementProperties = new MovementProperties();
    if ( m_ControlRenderer == null )
    { 
      m_ControlRenderer    = gameObject.AddComponent<DebugArrowRenderer>();
      m_MovementRenderer   = gameObject.AddComponent<DebugArrowRenderer>();
      m_MaxSpeedRenderer   = gameObject.AddComponent<DebugArrowRenderer>();

      m_ControlRenderer.m_Color  = m_Color;
      m_MovementRenderer.m_Color = m_Color;
      m_MaxSpeedRenderer.m_Color = m_SpeedColor;

      m_ControlOutline  = gameObject.AddComponent<DebugCircleRenderer>();
      m_MovementOutline = gameObject.AddComponent<DebugCircleRenderer>();

      m_ControlOutline.m_Color   = m_Color;
      m_MovementRenderer.m_Color = m_Color;
    }
  }

  private void OnDisable()
  {
    if ( m_ControlRenderer != null )
    {
      m_ControlRenderer.enabled  = false;
      m_MovementRenderer.enabled = false;
      m_ControlOutline.enabled = false;
      m_MovementOutline.enabled = false;
    }
  }

  void Update()
  {
    if ( m_ControlRenderer != null )
    {
      // do all of this in screen space, then convert to world
      Vector2 control_arrow_start = m_ScreenPosition + new Vector2( m_ArrowSizePx, -m_ArrowSizePx );
      Vector2 control_arrow_end   = control_arrow_start + m_MovementProperties.m_ControlDir * m_ArrowSizePx;

      Vector2 radius_pt = new Vector2( m_ArrowSizePx, 0f );
      radius_pt         = Camera.main.ScreenToWorldPoint( radius_pt ) - Camera.main.ScreenToWorldPoint( Vector3.zero );

      Console.WriteLine( radius_pt.x );
      
      // transform to world
      m_ControlRenderer.m_Start = Camera.main.ScreenToWorldPoint( control_arrow_start );
      m_ControlRenderer.m_End   = Camera.main.ScreenToWorldPoint( control_arrow_end );
      m_ControlOutline.m_Center = Camera.main.ScreenToWorldPoint( control_arrow_start );
      m_ControlOutline.m_Radius = radius_pt.x;

      float movement_vec_len = m_MovementProperties.m_Movement.magnitude;

      Vector2 movement_arrow_start = m_ScreenPosition + new Vector2( m_ArrowSizePx * 3.5f, -m_ArrowSizePx );
      Vector2 movement_arrow_vec   = m_MovementProperties.m_Movement
                                   * ( (movement_vec_len == 0f )
                                      ? 0f 
                                      : 1f / movement_vec_len 
                                     ) 
                                   * m_ArrowSizePx;

      Vector2 movement_arrow_end   = movement_arrow_start + movement_arrow_vec;

      m_MovementRenderer.m_Start = Camera.main.ScreenToWorldPoint( movement_arrow_start );
      m_MovementRenderer.m_End   = Camera.main.ScreenToWorldPoint( movement_arrow_end );
      m_MovementOutline.m_Center = Camera.main.ScreenToWorldPoint( movement_arrow_start );
      m_MovementOutline.m_Radius = radius_pt.x;
      
      Vector2 speed_arrow_end    = movement_arrow_start + movement_arrow_vec * ( movement_vec_len / m_MovementProperties.m_MaxSpeed );

      m_MaxSpeedRenderer.m_Start = m_MovementRenderer.m_Start;
      m_MaxSpeedRenderer.m_End   = Camera.main.ScreenToWorldPoint( speed_arrow_end );
    }
  }
}
