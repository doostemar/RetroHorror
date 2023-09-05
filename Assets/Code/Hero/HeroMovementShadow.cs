using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeroMovementShadow : MonoBehaviour
{
  public float m_Velocity = 2f;

  private HeroSelfEventSystem m_HeroEvents;
  private List<Tilemap>       m_CollisionTilemaps;
  private bool                m_PrevIsMoving;

  // Start is called before the first frame update
  void Start()
  {
    m_HeroEvents = GetComponent<HeroSelfEventSystem>();
    m_HeroEvents.OnHeroSelfEvent += OnSelfEvent;

    m_PrevIsMoving = false;

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

  public void OnSelfEvent(HeroSelfEvent self_event)
  {
    if (self_event.m_Type == HeroSelfEvent.EventType.HeroStateCasting)
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
    Vector3 next_pos = GetNextPosFromInput( player_input );

    // check collision
    if ( CheckForCollisionAt( next_pos ) )
    {
      // allow player to slide past
      Vector3 no_vertical = player_input;
      no_vertical.y = 0f;
      Vector3 next_pos_no_vert = GetNextPosFromInput( no_vertical );

      Vector3 no_horizontal = player_input;
      no_horizontal.x = 0f;
      Vector3 next_pos_no_hor = GetNextPosFromInput( no_horizontal );

      if ( CheckForCollisionAt( next_pos_no_vert ) == false )
      {
        next_pos = next_pos_no_vert;
      }
      else if ( CheckForCollisionAt( next_pos_no_hor ) == false )
      {
        next_pos = next_pos_no_hor;
      }
      else
      {
        next_pos = transform.position;
      }
    }

    bool is_moving = next_pos != transform.position;
    transform.position = next_pos;

    // Handle state stuff
    if ( is_moving && !m_PrevIsMoving )
    {
      HeroSelfEvent hero_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
      hero_event.m_Type = HeroSelfEvent.EventType.HeroStateMoving;  
      m_HeroEvents.RaiseEvent(hero_event);
    }
    else if ( !is_moving && m_PrevIsMoving )
    {
      HeroSelfEvent hero_event = ScriptableObject.CreateInstance<HeroSelfEvent>();
      hero_event.m_Type = HeroSelfEvent.EventType.HeroStateIdle;
      m_HeroEvents.RaiseEvent(hero_event);
    }

    m_PrevIsMoving = is_moving;
  }

  Vector3 GetNextPosFromInput( Vector3 input )
  {
    return transform.position + (input * m_Velocity * Time.deltaTime);
  }

  bool CheckForCollisionAt( Vector3 pos )
  {
    foreach ( var tilemap in m_CollisionTilemaps )
    {
      Vector3Int        tile_pos = tilemap.WorldToCell( pos );
      Tile.ColliderType collider = tilemap.GetColliderType( tile_pos );

      if ( collider != Tile.ColliderType.None )
      { 
        return true;
      }
    }
    return false;
  }
}
