using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBasicHeroFollower : MonoBehaviour
{
  public GameObject m_Hero;
  public Vector2    m_MovementPadding;

  void Update()
  {
     
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color =  new Color( 177f / 255f, 52f / 255f, 63f / 255f );

    Vector3 pos  = transform.position;
    pos.z = 0f;
    float x_offset = Camera.main.orthographicSize * Camera.main.aspect;
    float y_offset = Camera.main.orthographicSize;

    x_offset -= m_MovementPadding.x;
    y_offset -= m_MovementPadding.y;

    Vector3[] list = { 
      pos + new Vector3( -x_offset, -y_offset, 0f ),
      pos + new Vector3(  x_offset, -y_offset, 0f ),
      pos + new Vector3(  x_offset,  y_offset, 0f ),
      pos + new Vector3( -x_offset,  y_offset, 0f )
    };

    Gizmos.DrawLineStrip( list, true );
  }
}
