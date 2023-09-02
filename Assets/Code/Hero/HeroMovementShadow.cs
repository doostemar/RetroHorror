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
    Vector3 player_input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;
    Vector3 next_pos = transform.position + ( player_input * m_Velocity * Time.deltaTime );

    // check collision
    bool has_collision = false;
    bool is_moving = false;
    foreach ( var tilemap in m_CollisionTilemaps )
    {
      Vector3Int        tile_pos = tilemap.WorldToCell( next_pos );
      Tile.ColliderType collider = tilemap.GetColliderType( tile_pos );

      if ( collider != Tile.ColliderType.None )
      {
        has_collision = true;
      }
    }
    
    if ( has_collision == false )
    {
      is_moving = player_input != Vector3.zero;
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
    }

    m_PrevIsMoving = is_moving;
  }
}
