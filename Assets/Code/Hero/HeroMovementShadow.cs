using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeroMovementShadow : MonoBehaviour
{
  public float m_Velocity    = 2f;
  public float m_SlideScaler = 0.5f;

  private HeroEventSystem m_HeroEvents;
  private BoxCollider2D   m_BoxCollider;
  private List<Tilemap>   m_CollisionTilemaps;
  private bool            m_PrevIsMoving;
  private Vector3         m_LastOffAxisMovementDirection;

  // Start is called before the first frame update
  void Start()
  {
    m_HeroEvents = GetComponent<HeroEventSystem>();
    m_HeroEvents.OnHeroEvent += OnSelfEvent;
    
    m_BoxCollider = GetComponent<BoxCollider2D>();

    m_PrevIsMoving = false;
    m_LastOffAxisMovementDirection = new Vector3( 1, 1, 0 ).normalized;

    GameObject[] tilemap_objects = GameObject.FindGameObjectsWithTag( "CollisionGrid" );
    m_CollisionTilemaps = new List<Tilemap>();
    foreach ( var tilemap_object in tilemap_objects )
    {
      Tilemap tm = tilemap_object.GetComponent<Tilemap>();
      if ( tm != null )
      {
        m_CollisionTilemaps.Add( tm );
      }
    }
  }

  public void OnSelfEvent(HeroEvent self_event)
  {
    if (self_event.m_Type == HeroEvent.EventType.HeroStateCasting)
    {
      enabled = false;
    }
    else
    {
      enabled = true;
    }
  }

  void Update()
  {
    Vector3 player_input = new Vector3( Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;
    Vector3 next_pos = transform.position + GetNextPosFromInput( player_input );

    if ( Mathf.Abs( player_input.x ) != 1f 
      && Mathf.Abs( player_input.y ) != 1f 
      && player_input != Vector3.zero )
    {
      m_LastOffAxisMovementDirection = player_input;
    }

    // check collision
    if ( CheckForCollisionAt( next_pos ) )
    {
      // allow player to slide past
      Vector3 h_movement = new Vector3( player_input.x, 0f, 0f );
      Vector3 next_pos_h_only = transform.position + GetNextPosFromInput( h_movement );

      Vector3 v_movement = new Vector3( 0f, player_input.y, 0f );
      Vector3 next_pos_v_only = transform.position + GetNextPosFromInput( v_movement );      

      if ( h_movement != Vector3.zero && CheckForCollisionAt( next_pos_h_only ) == false )
      {
        next_pos = next_pos_h_only;
      }
      else if ( v_movement != Vector3.zero && CheckForCollisionAt( next_pos_v_only ) == false )
      {
        next_pos = next_pos_v_only;
      }
      else
      {
        // use stored value for slower sliding sliding
        Vector3 mem_h_movment = new Vector3( m_LastOffAxisMovementDirection.x, 0f, 0f );
        Vector3 next_pos_h_mem = transform.position + GetNextPosFromInput( mem_h_movment ) * m_SlideScaler;

        Vector3 mem_v_movement = new Vector3( 0f, m_LastOffAxisMovementDirection.y, 0f );
        Vector3 next_pos_v_mem = transform.position + GetNextPosFromInput( mem_v_movement ) * m_SlideScaler;

        if ( next_pos_h_mem != Vector3.zero && CheckForCollisionAt( next_pos_h_mem ) == false )
        {
          next_pos = next_pos_h_mem;
        }
        else if ( next_pos_v_mem != Vector3.zero && CheckForCollisionAt( next_pos_v_mem ) == false )
        {
          next_pos = next_pos_v_mem;
        }
        else
        {
          next_pos = transform.position;
        }
      }
    }

    bool is_moving = next_pos != transform.position;
    transform.position = next_pos;

    // Handle state stuff
    if ( is_moving && !m_PrevIsMoving )
    {
      HeroEvent hero_event = ScriptableObject.CreateInstance<HeroEvent>();
      hero_event.m_Type = HeroEvent.EventType.HeroStateMoving;  
      m_HeroEvents.RaiseEvent(hero_event);
    }
    else if ( !is_moving && m_PrevIsMoving )
    {
      HeroEvent hero_event = ScriptableObject.CreateInstance<HeroEvent>();
      hero_event.m_Type = HeroEvent.EventType.HeroStateIdle;
      m_HeroEvents.RaiseEvent(hero_event);
    }

    m_PrevIsMoving = is_moving;
  }

  Vector3 GetNextPosFromInput( Vector3 input )
  {
    return input * m_Velocity * Time.deltaTime;
  }

  bool CheckForCollisionAt( Vector3 pos )
  {
    foreach ( var tilemap in m_CollisionTilemaps )
    {
      // check each edge of bounding rect
      Vector3[] box_corners = {
        m_BoxCollider.offset + m_BoxCollider.size / 2f,
        m_BoxCollider.offset - m_BoxCollider.size / 2f,
        m_BoxCollider.offset + new Vector2(  m_BoxCollider.size.x, -m_BoxCollider.size.y ) / 2f,
        m_BoxCollider.offset + new Vector2( -m_BoxCollider.size.x,  m_BoxCollider.size.y ) / 2f
      };

      for ( int i_corner = 0; i_corner < box_corners.Length; ++i_corner )
      {
        Vector3 corner = pos + box_corners[i_corner ];

        Vector3Int        tile_pos = tilemap.WorldToCell( corner );
        Tile.ColliderType collider = tilemap.GetColliderType( tile_pos );

        if ( collider != Tile.ColliderType.None )
        {
          return true;
        }
      }
    }
    return false;
  }
}
