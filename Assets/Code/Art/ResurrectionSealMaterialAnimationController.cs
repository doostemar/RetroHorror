using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionSealMaterialAnimationController : MonoBehaviour
{
  public Color m_Color;
  public float m_Height;
  public float m_WiggleHeight;
  public float m_WiggleSpeed;
  public Color m_HighlightColor;

  private Material m_Mat;
  private int      m_ColorId;
  private int      m_HeightId;
  private int      m_WiggleHeightId;
  private int      m_WiggleSpeedId;
  private int      m_HighlightColorId;

  private void Start()
  {
    List<Material> materials = new List<Material>();
    GetComponent<SpriteRenderer>().GetMaterials( materials );

    if ( materials.Count > 0 )
    {
      m_Mat = materials[0];
    }

    m_ColorId          = Shader.PropertyToID( "_Color" );
    m_HeightId         = Shader.PropertyToID( "_Height" );
    m_WiggleHeightId   = Shader.PropertyToID( "_WiggleHeight" );
    m_WiggleSpeedId    = Shader.PropertyToID( "_WiggleSpeed" );
    m_HighlightColorId = Shader.PropertyToID( "_Highlight_Color" );

    SetProperties();
  }

  private void Update()
  {
    SetProperties();
  }

  private void SetProperties()
  {
    m_Mat.SetColor( m_ColorId,          m_Color );
    m_Mat.SetFloat( m_HeightId,         m_Height );
    m_Mat.SetFloat( m_WiggleHeightId,   m_WiggleHeight );
    m_Mat.SetFloat( m_WiggleSpeedId,    m_WiggleSpeed );
    m_Mat.SetColor( m_HighlightColorId, m_HighlightColor );
  }
}
