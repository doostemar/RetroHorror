using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BotEnemy : MonoBehaviour
{
  public GameObject m_CorpsePrefab;
  public float      m_AggroRange;
  public float      m_AttackRange;

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
    m_ArrivedAtTarget = false;
    m_State           = State.Idle;

    m_EnemyChannel.OnEnemyEvent += ( BotEnemyEvent evt ) =>
    {
      if ( evt.m_Type == BotEnemyEvent.Type.AttackFinished )
      {
        m_AttackFinished = true;
      }
    };
  }

  void Update()
  {
    if ( m_BotChannel == null )
    {
      m_BotChannel = GetComponent<BotChannel>();
      m_BotChannel.OnMoveEvent += OnBotMoveEvent;
    }

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

  void OnBotMoveEvent( BotMoveEvent evt )
  {
    if ( evt.m_Type == BotMoveEvent.Type.Arrived )
    {
      m_ArrivedAtTarget = true;
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

  bool IsUnitInAggroRange( GameObject unit )
  {
    float dist_sqr = ( unit.transform.position - transform.position ).sqrMagnitude;
    return dist_sqr < m_AggroRange * m_AggroRange;
  }

  bool IsUnitInAttackRange( GameObject unit )
  {
    float dist_sqr = ( unit.transform.position - transform.position ).sqrMagnitude;
    return dist_sqr < m_AttackRange * m_AttackRange;
  }

  void HandleAggro()
  {
    BotMoveEvent move_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
    move_evt.m_TargetPosition = m_AggroTarget.transform.position;
    move_evt.m_Type           = BotMoveEvent.Type.Move;
    m_BotChannel.RaiseMoveEvent( move_evt );

    TransitionToMovingToTarget();
  }

  void HandleMovingToAggroTarget()
  {
    if ( IsUnitInAttackRange( m_AggroTarget ) )
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
    UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.forward, m_AggroRange );

    UnityEditor.Handles.color = Color.red;
    UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.forward, m_AttackRange );
  }
}
