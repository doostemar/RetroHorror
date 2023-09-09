using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
  private const float kEpsilon = 0.00001f;

  public float m_MoveSpeed;

  private BotChannel m_Channel;
  public BotChannel Channel
  {
    get { return m_Channel; }
  }

  enum State
  {
    Idle,
    Moving
  };

  private Vector2           m_TargetPosition;
  private PathfindingSystem m_PathSystem;
  private State             m_State;
  private List<PathNode>    m_Path;
  private int               m_PathIdx; // which element in the path we're moving toward

  void Start()
  {
    m_Channel = gameObject.AddComponent<BotChannel>();
    m_State = State.Idle;
    m_PathSystem = Game.GetGameController().GetComponent<PathfindingSystem>();
    m_Channel.OnMoveEvent += OnMoveEvent;
  }

  void OnMoveEvent( BotMoveEvent evt )
  {
    switch ( evt.m_Type )
    {
      case BotMoveEvent.Type.Move:
        m_State          = State.Moving; // todo: if no valid path, this probably isn't correct
        m_TargetPosition = evt.m_TargetPosition;
        m_Path = m_PathSystem.FindPath( transform.position, evt.m_TargetPosition );
        m_PathIdx = 0;
        break;
      case BotMoveEvent.Type.Pause:
        m_State = State.Idle;
        break;
      case BotMoveEvent.Type.Resume:
        m_State = State.Moving;
        break;
      case BotMoveEvent.Type.Stop:
        TransitionToStopped();
        break;
      default:
        break;
    }
  }

  void TransitionToStopped()
  {
    m_State = State.Idle;
    m_Path = null;
    m_PathIdx = -1;
  }

  void Update()
  {
    if ( m_State == State.Moving )
    {
      Vector3 target_pos = new Vector3( m_TargetPosition.x, m_TargetPosition.y, transform.position.z );
      if ( m_Path != null )
      {
        if ( m_PathIdx < m_Path.Count )
        {
          PathNode path = m_Path[ m_PathIdx ];
          target_pos = new Vector3( path.WorldPos.x, path.WorldPos.y, transform.position.z );
        }
        Vector3 to_target = target_pos - transform.position;

        float move_incr = m_MoveSpeed * Time.deltaTime;

        if ( to_target.sqrMagnitude <= move_incr * move_incr )
        {
          transform.position = target_pos;

          if ( m_PathIdx < m_Path.Count )
          {
            m_PathIdx++;
          }
          else
          {
            BotMoveEvent arrived_evt = ScriptableObject.CreateInstance<BotMoveEvent>();
            arrived_evt.m_Type = BotMoveEvent.Type.Arrived;
            m_Channel.RaiseMoveEvent( arrived_evt );

            TransitionToStopped();
          }
        }
        else
        {
          Vector3 movement_dir = to_target.normalized;
          transform.position = transform.position + ( movement_dir * m_MoveSpeed * Time.deltaTime );
        }
      }
    }
  }
}
