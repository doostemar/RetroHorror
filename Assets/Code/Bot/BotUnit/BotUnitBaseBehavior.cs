using System;
using UnityEngine;

public class BotUnitBaseBehavior : MonoBehaviour
{
  public GameObject m_CorpsePrefab;
  public GameObject m_DeathParticleSystem;
  public float      m_AggroRange;
  public Vector2    m_RangeCenterOffset;

  public float      m_AttackRange;
  public Vector2    m_AttackTargetPosition;

  private enum State
  {
    Idle,
    MovingToAggroTarget,
    Attacking,
    ReturningToPosition,
  }

  private State          m_State;
  private GameObject     m_AggroTarget;
  private BotChannel     m_BotChannel;
  private BotUnitChannel m_UnitChannel;
  private bool           m_ArrivedAtTarget;
  private bool           m_AttackFinished;
  private Vector2        m_DirectedPosition;

  void Start()
  {
    m_UnitChannel     = GetComponent<BotUnitChannel>();
    m_BotChannel      = GetComponent<BotChannel>();
    m_ArrivedAtTarget = false;
    m_State           = State.Idle;

    m_UnitChannel.OnUnitEvent += OnUnitEvent;
    m_BotChannel.OnMoveEvent  += OnBotMoveEvent;
    m_DirectedPosition = transform.position;

    HealthChannel health_channel = GetComponent<HealthChannel>();
    health_channel.OnHealthEvent += OnHealthEvent;
  }

  private void OnDestroy()
  {
    m_BotChannel.OnMoveEvent  -= OnBotMoveEvent;
    m_UnitChannel.OnUnitEvent -= OnUnitEvent;
  }

  void OnBotMoveEvent( BotMoveEvent evt )
  {
    if ( evt.m_Type == BotMoveEvent.Type.Move )
    {
      m_DirectedPosition = evt.m_TargetPosition;
    }
    else if ( evt.m_Type == BotMoveEvent.Type.Arrived )
    {
      m_ArrivedAtTarget = true;
    }
  }

  void OnUnitEvent( BotUnitEvent evt )
  {
    if ( evt.m_Type == BotUnitEvent.Type.AttackFinished )
    {
      m_AttackFinished = true;
    }
  }

  void OnHealthEvent( HealthEvent evt )
  {
    if ( evt.m_Type == HealthEvent.Type.Dead )
    {
      Quaternion direction = Quaternion.LookRotation( evt.m_Direction );
      if ( m_DeathParticleSystem != null )
      {
        Instantiate( m_DeathParticleSystem, transform.position, direction );
      }
      if ( m_CorpsePrefab != null )
      {
        Instantiate( m_CorpsePrefab, transform.position, Quaternion.identity );
      }
      Destroy( gameObject );
    }
  }

  void Update()
  {
    switch ( m_State )
    {
      case State.Idle:
        HandleIdle();
        break;
      case State.MovingToAggroTarget:
        HandleMovingToAggroTarget();
        break;
      case State.Attacking:
        HandleAttacking();
        break;
      case State.ReturningToPosition:
        HandleReturningToPosition();
        break;
    }
  }

  void TransitionToIdle()
  {
    m_State = State.Idle;
  }

  void TransitionToMovingToTarget( GameObject target )
  {
    m_State           = State.MovingToAggroTarget;
    m_AggroTarget     = target;
    m_ArrivedAtTarget = false;

    Collider2D enemy_collider = m_AggroTarget.GetComponent<Collider2D>();
    Vector2 closest_enemy_pos = enemy_collider.ClosestPoint( transform.position );

    BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
    move_evt.m_TargetPosition = closest_enemy_pos - m_AttackTargetPosition;
    move_evt.m_Type           = BotMoveEvent.Type.Move;
    m_BotChannel.RaiseMoveEvent( move_evt );
  }

  void TransitionToAttacking()
  {
    {
      BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
      move_evt.m_Type = BotMoveEvent.Type.Stop;

      m_BotChannel.RaiseMoveEvent( move_evt );
    }

    {
      BotUnitEvent evt = ScriptableObject.CreateInstance<BotUnitEvent>();
      evt.m_Type = BotUnitEvent.Type.Attack;
      m_UnitChannel.RaiseUnitEvent( evt );
    }

    BotMoveEvent dir_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
    if ( m_AggroTarget.transform.position.x > transform.position.x )
    {
      dir_evt.m_Type = BotMoveEvent.Type.DirectionRight;
    }
    else
    {
      dir_evt.m_Type = BotMoveEvent.Type.DirectionLeft;
    }
    m_BotChannel.RaiseMoveEvent( dir_evt );

    m_AttackFinished = false;
    m_State = State.Attacking;
  }

  void TransitionToReturnToPosition()
  {
    m_State = State.ReturningToPosition;
    m_ArrivedAtTarget = false;

    BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
    move_evt.m_TargetPosition = m_DirectedPosition;
    move_evt.m_Type           = BotMoveEvent.Type.Move;
    m_BotChannel.RaiseMoveEvent( move_evt );
  }

  void HandleIdle()
  {
    // it may end up being very slow to look through every unit
    GameObject[] enemy_units = GameObject.FindGameObjectsWithTag( "EnemyUnit" );

    foreach ( var unit in enemy_units )
    {
      if ( IsUnitInAggroRange( unit ) )
      {
        // todo: handle line of sight?
        TransitionToMovingToTarget( unit );
        return;
      }
    }
  }

  bool IsUnitInAggroRange( GameObject unit )
  {
    Vector2 calc_center = GetAggroCenter();
    Vector2 closest_pt  = unit.GetComponent<Collider2D>().ClosestPoint( calc_center );

    float dist_sqr = ( closest_pt - calc_center ).sqrMagnitude;
    return dist_sqr < m_AggroRange * m_AggroRange;
  }

  bool IsUnitInAttackRange( GameObject unit )
  {
    Tuple<Vector3, Vector3> calc_centers = GetAttackTargetCenters();

    Collider2D unit_col = unit.GetComponent<Collider2D>();
    Vector3 pt1 = unit_col.ClosestPoint( calc_centers.Item1 );
    Vector3 pt2 = unit_col.ClosestPoint( calc_centers.Item2 );

    float dist_sqr1 = ( pt1 - calc_centers.Item1 ).sqrMagnitude;
    float dist_sqr2 = ( pt2 - calc_centers.Item2 ).sqrMagnitude;
    return dist_sqr1 < m_AttackRange * m_AttackRange || dist_sqr2 < m_AttackRange * m_AttackRange;
  }

  void HandleMovingToAggroTarget()
  {
    if ( m_AggroTarget == null ) // target has died
    {
      TransitionToReturnToPosition();
    }
    else if ( IsUnitInAttackRange( m_AggroTarget ) )
    {
      TransitionToAttacking();
    }
    else if ( m_ArrivedAtTarget )
    {
      if ( IsUnitInAggroRange( m_AggroTarget ) )
      {
        TransitionToMovingToTarget( m_AggroTarget );
      }
      else
      {
        TransitionToReturnToPosition();
      }
    }
  }

  void HandleReturningToPosition()
  {
    GameObject[] enemy_units = GameObject.FindGameObjectsWithTag( "EnemyUnit" );

    foreach ( var unit in enemy_units )
    {
      if ( IsUnitInAggroRange( unit ) )
      {
        // todo: handle line of sight?
        TransitionToMovingToTarget( unit );
        return;
      }
    }

    if ( m_ArrivedAtTarget )
    {
      TransitionToIdle();
    }
  }

  void HandleAttacking()
  {
    if ( m_AttackFinished )
    {
      if ( m_AggroTarget != null )
      {
        TransitionToMovingToTarget( m_AggroTarget );
      }
      else
      {
        TransitionToReturnToPosition();
      }
    }
  }

  Vector3 GetAggroCenter()
  {
    return m_DirectedPosition + m_RangeCenterOffset;
  }

  Tuple<Vector3, Vector3> GetAttackTargetCenters()
  {
    return Tuple.Create( transform.position + new Vector3(  m_AttackTargetPosition.x, m_AttackTargetPosition.y ),
                         transform.position + new Vector3( -m_AttackTargetPosition.x, m_AttackTargetPosition.y )
           );
  }

  private void OnDrawGizmosSelected()
  {
    UnityEditor.Handles.color = Color.green;
    UnityEditor.Handles.DrawWireDisc( transform.position + new Vector3( m_RangeCenterOffset.x, m_RangeCenterOffset.y, 0f ), Vector3.forward, m_AggroRange );

    UnityEditor.Handles.color = Color.red;
    Tuple<Vector3, Vector3> centers = GetAttackTargetCenters();
    UnityEditor.Handles.DrawWireDisc( centers.Item1, Vector3.forward, m_AttackRange );
    UnityEditor.Handles.DrawWireDisc( centers.Item2, Vector3.forward, m_AttackRange );
  }
}
