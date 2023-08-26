using System;
using UnityEngine;

public class DebugArrowRenderer : MonoBehaviour
{
  LineRenderer m_LineAsset;

  LineRenderer m_Line1;
  LineRenderer m_Line2;
  LineRenderer m_Line3;

  public Color   m_Color;
  public Vector2 m_Start;
  public Vector2 m_End;
  public float   m_ArrowAngle   = 20f;
  public float   m_ArrowPctSize = 0.2f;

  private void OnEnable()
  {
    if ( m_LineAsset == null )
    {
      // Must have "DebugLineRenderer" set up in Resources folder
      m_LineAsset = Resources.Load<LineRenderer>( "DebugLineRenderer" );
    }
    m_Line1 = Instantiate( m_LineAsset, DebugCommon.GetDebugObject() );
    m_Line2 = Instantiate( m_LineAsset, DebugCommon.GetDebugObject() );
    m_Line3 = Instantiate( m_LineAsset, DebugCommon.GetDebugObject() );

    m_Line1.positionCount = 2;
    m_Line2.positionCount = 2;
    m_Line3.positionCount = 2;
  }

  private void OnDisable()
  {
    Destroy(m_Line1);
    Destroy(m_Line2);
    Destroy(m_Line3);
  }

  void Update()
  {
    Vector2 dir     = m_End - m_Start;
    float len       = dir.magnitude;

    float arrow_len = len * m_ArrowPctSize;

    float arrow_angle_rad = Mathf.Deg2Rad * m_ArrowAngle;
    Vector2 arrow_1 = Vec2Math.Rotate2D( -( dir / len ),  arrow_angle_rad ) * arrow_len;
    Vector2 arrow_2 = Vec2Math.Rotate2D( -( dir / len ), -arrow_angle_rad ) * arrow_len;

    arrow_1 += m_End;
    arrow_2 += m_End;


    m_Line1.startColor = m_Color;
    m_Line1.endColor   = m_Color;
    m_Line2.startColor = m_Color;
    m_Line2.endColor   = m_Color;
    m_Line3.startColor = m_Color;
    m_Line3.endColor   = m_Color;
    m_Line1.SetPositions( new Vector3[] { m_Start, m_End } );

    if ( len > 0 )
    { 
      m_Line2.SetPositions( new Vector3[] { m_End, arrow_1 } );
      m_Line3.SetPositions( new Vector3[] { m_End, arrow_2 } );
    }
    else
    {
      m_Line2.SetPositions( new Vector3[] { Vector3.zero, Vector3.zero } );
      m_Line3.SetPositions( new Vector3[] { Vector3.zero, Vector3.zero } );
    }
  }
}
