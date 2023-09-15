using UnityEngine;

public class BotEnemy : MonoBehaviour
{
  public GameObject m_CorpsePrefab;
  public float      m_AggroRange;
  public Vector2    m_RangeCenterOffset;

  public float      m_AttackRange;
  public Vector2    m_AttackTargetPosition;

  private enum State
  {
    Idle,
    HasAggro,
    MovingToAggroTarget,
    Attacking,
  }

  private State           m_State;
  private GameObject      m_AggroTarget;
  private BotChannel      m_BotChannel;
  private BotEnemyChannel m_EnemyChannel;
  private bool            m_ArrivedAtTarget;
  private bool            m_AttackFinished;

  private void Start()
  {
    m_EnemyChannel    = GetComponent<BotEnemyChannel>();
    m_BotChannel      = GetComponent<BotChannel>();
    m_ArrivedAtTarget = false;
    m_State           = State.Idle;

    m_EnemyChannel.OnEnemyEvent += OnEnemyEvent;
    m_BotChannel.OnMoveEvent    += OnBotMoveEvent;
  }

  void OnDestroy()
  {
    m_BotChannel.OnMoveEvent    -= OnBotMoveEvent;
    m_EnemyChannel.OnEnemyEvent -= OnEnemyEvent;
  }

  void OnBotMoveEvent( BotMoveEvent evt )
  {
    if ( evt.m_Type == BotMoveEvent.Type.Arrived )
    {
      m_ArrivedAtTarget = true;
    }
  }

  void OnEnemyEvent( BotEnemyEvent evt )
  {
    if ( evt.m_Type == BotEnemyEvent.Type.AttackFinished )
    {
      m_AttackFinished = true;
    }
  }

  void Update()
  {
    switch ( m_State )
    {
      case State.Idle:
        HandleIdle();
        break;
      case State.HasAggro:
        HandleAggro();
        break;
      case State.MovingToAggroTarget:
        HandleMovingToAggroTarget();
        break;
      case State.Attacking:
        HandleAttacking();
        break;
    }
  }

  void TransitionToIdle()
  {
    m_State = State.Idle;
  }

  void TransitionToAggro( GameObject target )
  {
    m_State       = State.HasAggro;
    m_AggroTarget = target;
  }

  void TransitionToMovingToTarget()
  {
    m_State = State.MovingToAggroTarget;
    m_ArrivedAtTarget = false;
  }

  void TransitionToAttacking()
  {
    {
      BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
      move_evt.m_Type = BotMoveEvent.Type.Stop;

      m_BotChannel.RaiseMoveEvent( move_evt );
    }

    {
      BotEnemyEvent evt = ScriptableObject.CreateInstance<BotEnemyEvent>();
      evt.m_Type = BotEnemyEvent.Type.Attack;
      m_EnemyChannel.RaiseEnemyEvent( evt );
    }
    m_AttackFinished = false;
    m_State = State.Attacking;
  }

  void HandleIdle()
  {
    // it may end up being very slow to look through every unit
    GameObject[] hero_units = GameObject.FindGameObjectsWithTag( "HeroUnit" );

    foreach ( var unit in hero_units )
    {
      if ( IsUnitInAggroRange( unit ) )
      {
        // todo: handle line of sight?
        TransitionToAggro( unit );
        return;
      }
    }
  }

  Vector3 GetAggroCenter()
  {
    return transform.position + new Vector3( m_RangeCenterOffset.x, m_RangeCenterOffset.y );
  }

  Vector3 GetAttackTargetCenter()
  {
    return transform.position + new Vector3( m_AttackTargetPosition.x, m_AttackTargetPosition.y );
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
    Vector2 calc_center = GetAttackTargetCenter();
    Vector2 closest_pt  = unit.GetComponent<Collider2D>().ClosestPoint( calc_center );
    float dist_sqr = ( closest_pt - calc_center ).sqrMagnitude;
    return dist_sqr < m_AttackRange * m_AttackRange;
  }

  void HandleAggro()
  {
    // target may have been killed
    if ( m_AggroTarget != null )
    {
      Collider2D enemy_collider = m_AggroTarget.GetComponent<Collider2D>();
      Vector2 closest_enemy_pos = enemy_collider.ClosestPoint( transform.position );

      BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
      move_evt.m_TargetPosition = closest_enemy_pos - m_AttackTargetPosition;
      move_evt.m_Type           = BotMoveEvent.Type.Move;
      m_BotChannel.RaiseMoveEvent( move_evt );

      TransitionToMovingToTarget();
    }
    else
    {
      TransitionToIdle();
    }
  }

  void HandleMovingToAggroTarget()
  {
    if ( m_AggroTarget == null ) // target has died
    {
      TransitionToIdle();
    }
    else if ( IsUnitInAttackRange( m_AggroTarget ) )
    {
      TransitionToAttacking();
    }
    else if ( m_ArrivedAtTarget )
    {
      if ( IsUnitInAggroRange( m_AggroTarget ) )
      {
        TransitionToAggro( m_AggroTarget );
      }
      else
      {
        TransitionToIdle();
      }
    }
  }

  void HandleAttacking()
  {
    if ( m_AttackFinished )
    {
      TransitionToAggro( m_AggroTarget );
    }
  }

  private void OnDrawGizmosSelected()
  {
    UnityEditor.Handles.color = Color.green;
    UnityEditor.Handles.DrawWireDisc( GetAggroCenter(), Vector3.forward, m_AggroRange );

    UnityEditor.Handles.color = Color.red;
    UnityEditor.Handles.DrawWireDisc( GetAttackTargetCenter(), Vector3.forward, m_AttackRange );
  }
}
