using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bot : MonoBehaviour
{
  private const float kEpsilon = 0.00001f;

  public float m_MoveSpeed;
  public bool  m_DebugDrawMovement;

  private BotChannel m_Channel;

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

  private struct Debug
  {
    public LineRenderer       m_LineAsset;
    public List<LineRenderer> m_BoxLines;
    public LineRenderer       m_PathLine;
  }
  Debug m_Debug;

  void Start()
  {
    m_Channel = gameObject.GetComponent<BotChannel>();
    m_State = State.Idle;
    m_PathSystem = Game.GetGameController().GetComponent<PathfindingSystem>();
    m_Channel.OnMoveEvent += OnMoveEvent;

    m_Debug.m_LineAsset = Resources.Load<LineRenderer>( "DebugLineRenderer" );
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
      HandleMoving();
    }

    HandleDebug();
  }

  void HandleMoving()
  {
    if ( m_Path != null )
    {
      Vector3 target_pos = m_Path[ m_Path.Count - 1 ].WorldPosition;
      if ( m_PathIdx < m_Path.Count )
      {
        PathNode path = m_Path[ m_PathIdx ];
        target_pos = new Vector3( path.WorldPosition.x, path.WorldPosition.y, transform.position.z );
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

  void HandleDebug()
  {
    DestroyDebugViz();
    if ( m_DebugDrawMovement )
    {
      if ( m_Path != null )
      {
        // first draw each cell
        m_Debug.m_BoxLines = new List<LineRenderer>();
        Vector2 tile_size = m_PathSystem.GetTileSize();
        foreach ( PathNode node in m_Path )
        {
          Vector2 ll_corner = m_PathSystem.GetGridWorldPosition( node.GridPosition );

          Vector3[] box_pts =
          {
            ll_corner,
            ll_corner + new Vector2( tile_size.x, 0 ),
            ll_corner + tile_size,
            ll_corner + new Vector2( 0, tile_size.y )
          };

          LineRenderer lr = GameObject.Instantiate( m_Debug.m_LineAsset, DebugCommon.GetDebugObject() );
          lr.positionCount = 4;
          lr.loop          = true;
          lr.startColor    = Color.yellow;
          lr.endColor      = Color.yellow;
          lr.SetPositions( box_pts );
          m_Debug.m_BoxLines.Add( lr );
        }

        Vector3[] path_pts = (from p in m_Path select new Vector3( p.WorldPosition.x, p.WorldPosition.y ) ).ToArray();

        m_Debug.m_PathLine = GameObject.Instantiate( m_Debug.m_LineAsset, DebugCommon.GetDebugObject() );
        m_Debug.m_PathLine.positionCount = path_pts.Length;
        m_Debug.m_PathLine.loop          = false;
        m_Debug.m_PathLine.startColor    = Color.magenta;
        m_Debug.m_PathLine.endColor      = Color.cyan;
        m_Debug.m_PathLine.SetPositions( path_pts );
      }
    }
  }

  void DestroyDebugViz()
  {
    if ( m_Debug.m_BoxLines != null )
    {
      foreach ( LineRenderer lr in m_Debug.m_BoxLines )
      {
        Destroy( lr );
      }
      m_Debug.m_BoxLines.Clear();
    }

    if ( m_Debug.m_PathLine != null )
    {
      Destroy( m_Debug.m_PathLine );
    }
  }
}
