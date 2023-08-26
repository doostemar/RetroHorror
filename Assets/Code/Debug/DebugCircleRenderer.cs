using UnityEngine;

public class DebugCircleRenderer : MonoBehaviour
{
  LineRenderer m_LineAsset;
  LineRenderer m_Line;

  public Color   m_Color;
  public Vector2 m_Center;
  public float   m_Radius;
  public int     m_PointCount = 15;

  private void OnEnable()
  {
    if ( m_LineAsset == null )
    { 
      // Must have "DebugLineRenderer" set up in Resources folder
      m_LineAsset = Resources.Load<LineRenderer>( "DebugLineRenderer" );
    }

    m_Line = Instantiate( m_LineAsset );
    m_Line.positionCount = m_PointCount;
    m_Line.loop = true;
  }

  private void OnDisable()
  {
    Destroy( m_Line );
  }

  private void Update()
  {
    Vector3[] pts = new Vector3[ m_PointCount ];
    float div = ( Mathf.PI * 2f ) / m_PointCount;

    for ( int i_pt = 0; i_pt < m_PointCount; ++i_pt )
    {
      pts[i_pt ] = m_Center + Vec2Math.Rotate2D( Vector2.right, div * i_pt ) * m_Radius;
    }

    m_Line.SetPositions( pts );
  }
}
