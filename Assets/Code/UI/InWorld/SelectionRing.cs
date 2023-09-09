using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRing : MonoBehaviour
{
  public GameObject   m_LineRendererPrefab;
  public float        m_Width;
  public float        m_Height;
  public int          m_PointCount;
  public Vector2      m_Offset;

  private GameObject   m_LineRendererInstance;
  private LineRenderer m_LineRenderer;

  void Start()
  {
    m_LineRendererInstance = Instantiate( m_LineRendererPrefab, transform );
    m_LineRenderer = m_LineRendererInstance.GetComponent<LineRenderer>();
  }

  public void OnEnable()
  {
    if ( m_LineRenderer != null )
    {
      m_LineRenderer.enabled = true;
    }
  }

  public void OnDisable()
  {
    m_LineRenderer.enabled = false;
  }

  private void Update()
  {
    Vector3[] pts = new Vector3[ m_PointCount ];
    float div = ( Mathf.PI * 2f ) / m_PointCount;

    Vector3 center = transform.position + new Vector3( m_Offset.x, m_Offset.y );

    for ( int i_pt = 0; i_pt < m_PointCount; ++i_pt )
    {
      Vector2 pt = MathHelper.Rotate2D( Vector2.right, div * i_pt );
      pt.x *= m_Width;
      pt.y *= m_Height;
      pts[ i_pt ] = center + new Vector3( pt.x, pt.y );
    }

    m_LineRenderer.positionCount = pts.Length;
    m_LineRenderer.SetPositions( pts );
  }
}
