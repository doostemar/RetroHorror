using System;
using UnityEngine;

public class DebugHeroMovement : MonoBehaviour
{
  DebugArrowRenderer m_ControlRenderer;
  DebugArrowRenderer m_MovementRenderer;
  DebugArrowRenderer m_MaxSpeedRenderer;
  DebugCircleRenderer m_ControlOutline;
  DebugCircleRenderer m_MovementOutline;

  DebugHistogramRenderer m_Histogram;
  int                    m_SpeedHistogramId;
  int                    m_ControlHistogramId;

  Color m_Color = Color.white;
  // green
  Color m_SpeedColor = new Color( 51f  / 255f,
                                  140f / 255f,
                                  27f  / 255f );
  // light green
  Color m_SpeedGraphColor   = Color.green;

  // light blue
  Color m_ControlGraphColor = new Color(
    52f / 255f , 155f / 255f, 235f / 255f
  );

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

      m_ControlOutline.m_Color  = m_Color;
      m_MovementOutline.m_Color = m_Color;

      m_Histogram = gameObject.AddComponent<DebugHistogramRenderer>();
      m_Histogram.m_MaxHistory = 500;
      m_Histogram.m_ScreenBounds = new Rect(m_ScreenPosition.x, m_ScreenPosition.y - m_ArrowSizePx * 2.1f - 65, m_ArrowSizePx * 4.5f, 65 );
      m_SpeedHistogramId   = m_Histogram.AddGraph( m_SpeedGraphColor );
      m_ControlHistogramId = m_Histogram.AddGraph( m_ControlGraphColor );
    }
    else
    {
      m_ControlRenderer.enabled  = true;
      m_MovementRenderer.enabled = true;
      m_ControlOutline.enabled   = true;
      m_MovementOutline.enabled  = true;
      m_Histogram.enabled        = true;
    }
  }

  private void OnDisable()
  {
    if ( m_ControlRenderer != null )
    {
      m_ControlRenderer.enabled  = false;
      m_MovementRenderer.enabled = false;
      m_ControlOutline.enabled   = false;
      m_MovementOutline.enabled  = false;
      m_Histogram.enabled        = false;
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

      m_Histogram.PushData( m_SpeedHistogramId, movement_vec_len );
      m_Histogram.PushData( m_ControlHistogramId, m_MovementProperties.m_ControlDir.magnitude );
    }
  }
}
