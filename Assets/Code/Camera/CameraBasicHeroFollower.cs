//#define PRINT_TRANSITIONS

using UnityEngine;

public class CameraBasicHeroFollower : MonoBehaviour
{
  const float kEpsilon   = 0.000001f;
  const float kEasingMax = 0.05f;

  public Vector2    m_MovementPadding;
  public float      m_MovementPaddingScaleWhenMoving = 0.8f;
  public float      m_WaitForCenterSeconds;

  [ Range( 0f, 1f ) ]
  public float      m_AnchorFactor;

  [ Range( 0f, kEasingMax ) ]
  public float      m_MovementEasing;

  //[ Range( 0f, kEasingMax ) ]
  //public float      m_AnchorMovementEasing;

  enum State
  {
    AtRest,                 // nothing happening, no movement

    Anchored,               // The hero is moving, but has not left the boundary region.
                            // The camera should have an elastic feel around the "center", which is wherever the camera sits when easing begins

    CoolingDown,            // No movement from hero, we have a small delay before we move the camera to focus on the hero

    MovingToBoundaryTarget, // Hero is outside of the screen boundaries and is still moving. We are moving the camera to catch up

    MovingToHeroTarget,     // Hero has been still for a certain amount of time (according to cooldown).
                            // At this point we move the camera to focus on the hero
  }

  private GameObject m_Hero;
  private Collider2D m_HeroCollider;
  private bool       m_IsHeroMoving;
  private State      m_State;
  private float      m_TimeSinceMoved;
  private Vector2    m_TargetPosition;
  private Vector2    m_AnchorPosition;
  private Bounds     m_Boundaries;

  //-------------------------------------------------------------------------------------
  private void Start()
  {
    m_Hero         = GameObject.FindGameObjectWithTag( "Hero" );
    m_HeroCollider = m_Hero.GetComponent<Collider2D>();
    m_IsHeroMoving = false;

    HeroSelfEventSystem hero_events = m_Hero.GetComponent<HeroSelfEventSystem>();

    hero_events.OnHeroSelfEvent += OnHeroEvent;

    m_State               = State.AtRest;
    transform.position    = new Vector3( m_Hero.transform.position.x, m_Hero.transform.position.y, transform.position.z );
  }

  //-------------------------------------------------------------------------------------
  void OnHeroEvent( HeroSelfEvent hero_event )
  {
    if ( hero_event.m_Type == HeroSelfEvent.EventType.HeroStateMoving
      || hero_event.m_Type == HeroSelfEvent.EventType.HeroStateCasting )
    {
      m_IsHeroMoving = true;
    }
    else if ( hero_event.m_Type == HeroSelfEvent.EventType.HeroStateIdle
           || hero_event.m_Type == HeroSelfEvent.EventType.HeroStateCastDone )
    {
      m_IsHeroMoving = false;
    }
  }

  //-------------------------------------------------------------------------------------
  void LateUpdate()
  {
    switch ( m_State )
    {
      case State.AtRest:
        HandleAtRest();
        break;
      case State.Anchored:
        HandleAnchored();
        break;
      case State.CoolingDown:
        HandleCooldown();
        break;
      case State.MovingToBoundaryTarget:
        HandleOutsideBoundary();
        break;
      case State.MovingToHeroTarget:
        HandleMoveToHero();
        break;
    }
  }

  //-------------------------------------------------------------------------------------
  void TransitionToAtRest()
  {    
    #if PRINT_TRANSITIONS
      Debug.Log("Camera Transition To AtRest" );
    #endif
    m_State = State.AtRest;
  }

  //-------------------------------------------------------------------------------------
  void TransitionToAnchored()
  {
    #if PRINT_TRANSITIONS
      Debug.Log("Camera Transition To Anchored" );
    #endif
    m_State          = State.Anchored;
    m_AnchorPosition = transform.position;
  }

  //-------------------------------------------------------------------------------------
  //void TransitionToCoolingDown()
  //{
  //  #if PRINT_TRANSITIONS
  //    Debug.Log("Camera Transition To Cooldown" );
  //  #endif
  //  m_State = State.CoolingDown;
  //  m_TimeSinceMoved = 0f;
  //}

  //-------------------------------------------------------------------------------------
  //void TransitionToMoveToHero()
  //{
  //  #if PRINT_TRANSITIONS
  //    Debug.Log("Camera Transition To MoveToHero");
  //  #endif
  //  m_State = State.MovingToHeroTarget;
  //  m_TargetPosition = m_Hero.transform.position;
  //}

  //-------------------------------------------------------------------------------------
  void TransitionToMovingToBoundary()
  {
    #if PRINT_TRANSITIONS
      Debug.Log("Camera Transition To MoveToBoundary" );
    #endif
    m_State      = State.MovingToBoundaryTarget;
  }

  //-------------------------------------------------------------------------------------
  void HandleAtRest()
  {
    if ( m_IsHeroMoving )
    {
      TransitionToAnchored();
    }
  }

  //-------------------------------------------------------------------------------------
  Vector2 GetHeroOutsideBoundsVec( Bounds bounds )
  {
    Bounds hero_bounds     = m_HeroCollider.bounds;
    Vector2 total_catchup  = Vector2.zero;
    if ( hero_bounds.max.y > bounds.max.y )
    {
      total_catchup.y += hero_bounds.max.y - bounds.max.y;
    }

    if ( hero_bounds.max.x > bounds.max.x )
    {
      total_catchup.x += hero_bounds.max.x - bounds.max.x;
    }

    if ( hero_bounds.min.y < bounds.min.y )
    {
      total_catchup.y += hero_bounds.min.y - bounds.min.y;
    }

    if ( hero_bounds.min.x < bounds.min.x )
    {
      total_catchup.x += hero_bounds.min.x - bounds.min.x;
    }

    return total_catchup;
  }

  //-------------------------------------------------------------------------------------
  private Bounds CalculateBounds( Vector2 relative_to, Vector2 padding )
  {
    Vector3 pos  = relative_to;
    Vector3 size = new Vector3(
      Camera.main.orthographicSize * Camera.main.aspect,
      Camera.main.orthographicSize,
      0f
    );

    size.x -= padding.x;
    size.y -= padding.y;

    return new Bounds( pos, size * 2f );
  }

  //-------------------------------------------------------------------------------------
  void HandleAnchored()
  {
    //if ( m_IsHeroMoving == false )
    //{
    //  TransitionToCoolingDown();
    //  return;
    //}

    if ( IsOutsideAnchoredBounds() )
    {
      TransitionToMovingToBoundary();
      return;
    }

    // want to have the camera move toward the hero base on how far it is from the anchor point
    Vector2 hero_off_anchor = new Vector2(m_Hero.transform.position.x, m_Hero.transform.position.y) - m_AnchorPosition;
    Vector2 new_pos         = m_AnchorPosition + hero_off_anchor * ( 1f - m_AnchorFactor );

    Vector2 cam_pos = transform.position;
    FrameMoveTo( new_pos, ref cam_pos );

    transform.position = new Vector3( cam_pos.x, cam_pos.y, transform.position.z );
  }

  //-------------------------------------------------------------------------------------
  bool IsOutsideAnchoredBounds()
  {
    Bounds  anchored_bounds   = CalculateBounds( m_AnchorPosition, m_MovementPadding );
    Vector2 outside_bounds = GetHeroOutsideBoundsVec( anchored_bounds );
    return outside_bounds.sqrMagnitude > kEpsilon;
  }

  //-------------------------------------------------------------------------------------
  void HandleCooldown()
  {
    //if ( m_IsHeroMoving )
    //{
    //  if ( IsOutsideAnchoredBounds() )
    //  {
    //    TransitionToMovingToBoundary();
    //    return;
    //  }
    //  else
    //  {
    //    TransitionToAnchored();
    //    return;
    //  }
    //}

    //m_TimeSinceMoved += Time.deltaTime;
    //if ( m_TimeSinceMoved > m_WaitForCenterSeconds )
    //{
    //  TransitionToMoveToHero();
    //}
  }

  //-------------------------------------------------------------------------------------
  void HandleOutsideBoundary()
  {
    //if ( m_IsHeroMoving == false )
    //{
    //  TransitionToMoveToHero();
    //}

    Bounds  boundaries = CalculateBounds( m_AnchorPosition, m_MovementPadding * m_MovementPaddingScaleWhenMoving );
    Vector2 outside_boundary_vec = GetHeroOutsideBoundsVec( boundaries );

    if ( outside_boundary_vec.sqrMagnitude < kEpsilon )
    {
      TransitionToAnchored();
    }

    Vector2 move_to_vec = outside_boundary_vec + new Vector2( transform.position.x, transform.position.y );
    Vector2 pos = transform.position;
    
    FrameMoveTo( move_to_vec, ref pos );
    transform.position = new Vector3( pos.x, pos.y, transform.position.z );

    FrameMoveTo( pos, ref m_AnchorPosition );
  }

  //-------------------------------------------------------------------------------------
  // returns true when it's arrived
  bool FrameMoveTo( Vector2 target_pos, ref Vector2 pos_to_move )
  {
    Vector2 target_vec = target_pos - pos_to_move;
    Vector2 move_vec   = target_vec * ( kEasingMax - m_MovementEasing );
    pos_to_move += move_vec;

    if ( (pos_to_move - target_pos).sqrMagnitude < kEpsilon )
    {
      pos_to_move = target_pos;
      return true;
    }

    return false;
  }

  //-------------------------------------------------------------------------------------
  void HandleMoveToHero()
  {
    m_AnchorPosition = transform.position;

    if ( m_IsHeroMoving )
    {
      if ( IsOutsideAnchoredBounds() )
      {
        TransitionToMovingToBoundary();
        return;
      }
      else
      {
        TransitionToAnchored();
        return;
      }
    }

    Vector2 pos = transform.position;
    bool arrived = FrameMoveTo( m_TargetPosition, ref pos );
    transform.position = new Vector3( pos.x, pos.y, transform.position.z );
    if ( arrived )
    {
      TransitionToAtRest();
    }
  }

  //-------------------------------------------------------------------------------------
  private void OnDrawGizmosSelected()
  {
    Gizmos.color =  new Color( 177f / 255f, 52f / 255f, 63f / 255f );
    Bounds bounds = CalculateBounds( transform.position, m_MovementPadding );

    Vector3[] list = { 
      new Vector3( bounds.min.x, bounds.min.y, 0f ),
      new Vector3( bounds.max.x, bounds.min.y, 0f ),
      new Vector3( bounds.max.x, bounds.max.y, 0f ),
      new Vector3( bounds.min.x, bounds.max.y, 0f )
    };

    Gizmos.DrawLineStrip( list, true );
  }
}
