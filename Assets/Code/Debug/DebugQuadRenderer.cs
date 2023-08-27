using UnityEngine;

public class DebugQuadRenderer : MonoBehaviour
{
  public Rect  m_Rect;
  public Color m_Color;

  MeshRenderer m_Renderer;
  MeshFilter   m_Filter;

  private void OnEnable()
  {
    transform.position = Vector3.zero;
    if ( m_Renderer == null )
    {
      m_Renderer = gameObject.AddComponent<MeshRenderer>();
      m_Renderer.sharedMaterial = new Material( Shader.Find("Sprites/Default") );
      m_Filter = gameObject.AddComponent<MeshFilter>();
    }
    else
    {
      m_Renderer.enabled = true;
    }
  }

  private void OnDisable()
  {
    m_Renderer.enabled = false;
  }

  private void Update()
  {
    Vector3[] pts = new Vector3[4]
    {
      new Vector3( m_Rect.min.x, m_Rect.min.y, 0f ),
      new Vector3( m_Rect.max.x, m_Rect.min.y, 0f ),
      new Vector3( m_Rect.max.x, m_Rect.max.y, 0f ),
      new Vector3( m_Rect.min.x, m_Rect.max.y, 0f )
    };

    int[] tris = new int[6]
    {
      0, 3, 1,
      3, 2, 1
    };
    Vector3[] normals = new Vector3[4]
    {
      -Vector3.forward,
      -Vector3.forward,
      -Vector3.forward,
      -Vector3.forward
    };
    Color[] background_colors = new Color[4]
    {
      m_Color,
      m_Color,
      m_Color,
      m_Color
    };

    Mesh mesh = new Mesh();
    mesh.vertices  = pts;
    mesh.triangles = tris;
    mesh.normals   = normals;
    mesh.colors    = background_colors;
    m_Filter.mesh = mesh;
  }
}
