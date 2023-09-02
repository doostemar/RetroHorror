using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBasicHeroFollower : MonoBehaviour
{
  const float kEasingMax = 0.05f;

  public Vector2    m_MovementPadding;
  public float      m_WaitForCenterSeconds;

  [ Range( 0f, kEasingMax ) ]
  public float      m_MovementEasing;

  enum State
  {
    AtRest,
    CoolingDown,
    MovingToBoundaryTarget,
    MovingToHeroTarget,
  }

  private GameObject m_Hero;
  private Collider2D m_HeroCollider;
  private State      m_State;
  private float      m_TimeSinceMoved;
  private Vector2    m_TargetPosition;

  private void Start()
  {
    m_Hero         = GameObject.FindGameObjectWithTag( "Hero" );
    m_HeroCollider = m_Hero.GetComponent<Collider2D>();

    HeroSelfEventSystem hero_events = m_Hero.GetComponent<HeroSelfEventSystem>();

    hero_events.OnHeroSelfEvent += OnHeroEvent;

    m_State               = State.CoolingDown;
    m_TimeSinceMoved      = 0f;
    m_TargetPosition      = m_Hero.transform.position;
  }

  void LateUpdate()
  {
    HandleBoundaries();
    HandleMovementCooldown();
    HandleMoveToTarget();
  }

  void OnHeroEvent( HeroSelfEvent hero_event )
  {
    if ( hero_event.m_Type == HeroSelfEvent.EventType.HeroStateMoving
      || hero_event.m_Type == HeroSelfEvent.EventType.HeroStateCasting )
    {
      m_State = State.AtRest;
    }
    else if ( hero_event.m_Type == HeroSelfEvent.EventType.HeroStateIdle
           || hero_event.m_Type == HeroSelfEvent.EventType.HeroStateCastDone )
    {
      TransitionToCoolingDown();
    }
  }

  void HandleBoundaries()
  {
    Bounds movement_bounds = CalculateBounds( m_MovementPadding );
    Bounds hero_bounds = m_HeroCollider.bounds;
    Vector2 total_catchup = Vector2.zero;
    if ( hero_bounds.max.y > movement_bounds.max.y )
    {
      total_catchup.y += hero_bounds.max.y - movement_bounds.max.y;
    }

    if ( hero_bounds.max.x > movement_bounds.max.x )
    {
      total_catchup.x += hero_bounds.max.x - movement_bounds.max.x;
    }

    if ( hero_bounds.min.y < movement_bounds.min.y )
    {
      total_catchup.y += hero_bounds.min.y - movement_bounds.min.y;
    }

    if ( hero_bounds.min.x < movement_bounds.min.x )
    {
      total_catchup.x += hero_bounds.min.x - movement_bounds.min.x;
    }

    if ( total_catchup.sqrMagnitude > 0.000001f )
    {
      m_TargetPosition = transform.position + new Vector3( total_catchup.x, total_catchup.y );
      m_State = State.MovingToBoundaryTarget;
    }
  }

  void TransitionToCoolingDown()
  {
    m_State = State.CoolingDown;
    m_TimeSinceMoved = 0f;
  }

  void HandleMovementCooldown()
  {
    if ( m_State == State.CoolingDown )
    {
      m_TimeSinceMoved += Time.deltaTime;
      if ( m_TimeSinceMoved > m_WaitForCenterSeconds )
      {
        m_State = State.MovingToHeroTarget;
        m_TargetPosition = m_Hero.transform.position;
      }
    }
  }

  void HandleMoveToTarget()
  {
    if ( m_State == State.MovingToHeroTarget || m_State == State.MovingToBoundaryTarget )
    {
      Vector2 target_vec = m_TargetPosition - new Vector2( transform.position.x, transform.position.y );
      Vector2 move_vec   = target_vec * ( kEasingMax - m_MovementEasing );
      transform.position += new Vector3( move_vec.x, move_vec.y, 0f );

      Vector2 transform_2d = transform.position;

      const float kEpsilon       = 0.0001f;
      const float kMoveToEpsilon = 0.000000001f;
      if ( move_vec.sqrMagnitude < kMoveToEpsilon || ( transform_2d - m_TargetPosition ).sqrMagnitude < kEpsilon )
      {
        transform.position = new Vector3( m_TargetPosition.x, m_TargetPosition.y, transform.position.z );

        if ( m_State == State.MovingToHeroTarget )
        { 
          m_State = State.AtRest;
        }
        else if ( m_State == State.MovingToBoundaryTarget )
        {
          TransitionToCoolingDown();
        }
      }
    }
  }

  private Bounds CalculateBounds( Vector2 padding )
  {
    Vector3 pos  = transform.position;
    Vector3 size = new Vector3(
      Camera.main.orthographicSize * Camera.main.aspect,
      Camera.main.orthographicSize,
      0f
    );

    size.x -= padding.x;
    size.y -= padding.y;

    return new Bounds( pos, size * 2f );
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color =  new Color( 177f / 255f, 52f / 255f, 63f / 255f );
    Bounds bounds = CalculateBounds( m_MovementPadding );

    Vector3[] list = { 
      new Vector3( bounds.min.x, bounds.min.y, 0f ),
      new Vector3( bounds.max.x, bounds.min.y, 0f ),
      new Vector3( bounds.max.x, bounds.max.y, 0f ),
      new Vector3( bounds.min.x, bounds.max.y, 0f )
    };

    Gizmos.DrawLineStrip( list, true );
  }
}
