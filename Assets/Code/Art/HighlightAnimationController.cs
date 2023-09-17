using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAnimationController : MonoBehaviour
{
  public Color m_HighlightColor;
  public bool  m_Highlighted;
  public float m_HighlightBrightness;

  private Material m_Mat;
  private int      m_HighlightColorId;
  private int      m_HighlightedId;
  private int      m_HighlightBrightnessId;

  void Start()
  {
    List<Material> materials = new List<Material>();
    GetComponent<SpriteRenderer>().GetMaterials( materials );

    if ( materials.Count > 0 )
    {
      m_Mat = materials[0];
    }

    m_HighlightColorId      = Shader.PropertyToID( "_HighlightColor" );
    m_HighlightedId         = Shader.PropertyToID( "_Highlighted" );
    m_HighlightBrightnessId = Shader.PropertyToID( "_HighlightBrightness" );

    SetProperties();
  }

  void Update()
  {
    SetProperties();
  }

  private void SetProperties()
  {
    m_Mat.SetColor( m_HighlightColorId,      m_HighlightColor );
    m_Mat.SetInt  ( m_HighlightedId,         m_Highlighted ? 1 : 0 );
    m_Mat.SetFloat( m_HighlightBrightnessId, m_HighlightBrightness );
  }
}
