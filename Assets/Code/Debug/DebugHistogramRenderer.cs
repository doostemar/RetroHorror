using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugHistogramRenderer : MonoBehaviour
{
  public int  m_MaxHistory   = 100;
  public Rect m_ScreenBounds = new Rect( 100, 100, 100, 60 );
  
  struct HistoricalData
  {
    public LineRenderer m_Graph;
    public Color        m_Color;
    public List<float>  m_History;
    public float        m_MaxValue;
  }

  LineRenderer         m_LineAsset;
  DebugQuadRenderer    m_QuadAsset;
  LineRenderer         m_Outline;
  List<HistoricalData> m_Data;
  DebugQuadRenderer    m_BackgroundRenderer;

  private void OnEnable()
  {
    if ( m_LineAsset == null )
    {
      // Must have "DebugLineRenderer" set up in Resources folder
      m_LineAsset = Resources.Load<LineRenderer>( "DebugLineRenderer" );
      m_QuadAsset = Resources.Load<DebugQuadRenderer>( "DebugQuadRenderer" );
      m_Data      = new List<HistoricalData>();
    }

    for ( int i_data = 0; i_data < m_Data.Count; ++i_data )
    {
      var data = m_Data[ i_data ];
      data.m_Graph = Instantiate( m_LineAsset, DebugCommon.GetDebugObject() );
      data.m_Graph.loop = false;
    }

    m_BackgroundRenderer = Instantiate( m_QuadAsset, DebugCommon.GetDebugObject() );
    m_BackgroundRenderer.m_Color = new Color( 0f, 0f, 0f, 0.6f );

    m_Outline = Instantiate( m_LineAsset, DebugCommon.GetDebugObject() );

    m_Outline.positionCount = 4;
    m_Outline.loop = true;
  }

  private void OnDisable()
  {
    Destroy(m_BackgroundRenderer);
    for ( int i_data = 0; i_data < m_Data.Count; ++i_data )
    {
      var data = m_Data[ i_data ];
      Destroy(data.m_Graph);
    }
  }

  public int AddGraph( Color color )
  {
    int id = m_Data.Count;

    HistoricalData data = new HistoricalData();
    if ( enabled )
    { 
      data.m_Graph = Instantiate( m_LineAsset, DebugCommon.GetDebugObject() );
    }
    data.m_Color    = color;
    data.m_MaxValue = 0.00001f;
    data.m_History  = new List<float>( m_MaxHistory );

    m_Data.Add( data );

    return id;
  }

  public void PushData( int id, float value )
  {
    HistoricalData data = m_Data[ id ];
    if ( data.m_History.Count == m_MaxHistory )
    {
      data.m_History.RemoveAt( 0 );
    }
    data.m_History.Add( value );
    data.m_MaxValue = Mathf.Max( data.m_MaxValue, value );
    m_Data[ id ] = data;
  }

  void Update()
  {
    // Draw background
    Vector2 outline_cam_ll = Camera.main.ScreenToWorldPoint(m_ScreenBounds.min);
    Vector2 outline_cam_ur = Camera.main.ScreenToWorldPoint(m_ScreenBounds.max);

    Vector3[] outline_pts = new Vector3[4] {
      new Vector3( outline_cam_ll.x, outline_cam_ll.y ),
      new Vector3( outline_cam_ur.x, outline_cam_ll.y ),
      new Vector3( outline_cam_ur.x, outline_cam_ur.y ),
      new Vector3( outline_cam_ll.x, outline_cam_ur.y )
    };

    m_BackgroundRenderer.m_Rect = new Rect( outline_cam_ll, outline_cam_ur - outline_cam_ll );

    // Draw each graph
    for ( int i_data = 0; i_data < m_Data.Count; ++i_data )
    {
      HistoricalData data = m_Data[ i_data ];
      float     x_interval = m_ScreenBounds.width / m_MaxHistory;
      float     x_start    = m_ScreenBounds.max.x - x_interval * data.m_History.Count;
      Vector3[] positions  = new Vector3[ data.m_History.Count ];

      for ( int i_sample = 0; i_sample < positions.Length; ++i_sample )
      {
        float   norm_height   = data.m_History[i_sample] / data.m_MaxValue;
        Vector2 screen_pos    = new Vector2(x_start + x_interval * i_sample, m_ScreenBounds.min.y + m_ScreenBounds.height * norm_height );
        positions[ i_sample ] = Camera.main.ScreenToWorldPoint( screen_pos );
        positions[ i_sample ].z = 0f;
      }

      data.m_Graph.positionCount = positions.Length;
      data.m_Graph.startColor    = data.m_Color;
      data.m_Graph.endColor      = data.m_Color;
      data.m_Graph.startWidth    = 0.01f;
      data.m_Graph.endWidth      = 0.01f;
      data.m_Graph.SetPositions( positions );
    }

    // Draw outline
    m_Outline.startColor = Color.white;
    m_Outline.endColor   = Color.white;
    m_Outline.SetPositions( outline_pts );
  }
}
