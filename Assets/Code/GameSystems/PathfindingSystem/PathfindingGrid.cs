using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class PathfindingGrid
{
  private Dictionary<Vector2Int, GridNode> m_Nodes;
  private List<Tilemap> m_CollisionTilemaps;

  //-------------------------------------------------------------------------------------
  public PathfindingGrid( List<Tilemap> collision_tilemaps )
  {
    m_Nodes = new Dictionary<Vector2Int, GridNode>();
    m_CollisionTilemaps = collision_tilemaps;
  }

  //-------------------------------------------------------------------------------------
  public Vector2Int WorldToGrid( Vector2 pos )
  {
    if ( m_CollisionTilemaps.Count > 0 )
    {
      Vector3Int cell_pos_3 = m_CollisionTilemaps[ 0 ].WorldToCell( pos );
      return new Vector2Int( cell_pos_3.x, cell_pos_3.y );
    }
    return Vector2Int.zero;
  }

  //-------------------------------------------------------------------------------------
  public void InitGridObject( Vector2Int position )
  {
    GridNode node = new GridNode( position );
    node.m_GCost = int.MaxValue;
    node.CalculateFCost();
    node.m_PreviousNode = null;
    m_Nodes[position] = node;
  }

  //-------------------------------------------------------------------------------------
  public enum WalkableState
  {
    Open,
    Closed,
    DiagonallyClosed
  }

  //-------------------------------------------------------------------------------------
  // We assume that from and to are adjacent
  public WalkableState FindWalkableState( GridNode from, GridNode to )
  {
    List< Vector3Int > check_positions = new List< Vector3Int >
    {
      new Vector3Int( to.GridPos.x, to.GridPos.y )
    };

    Vector2Int rel_pos = to.GridPos - from.GridPos;
    Vector2Int flat_x = from.GridPos + new Vector2Int( rel_pos.x, 0 );
    if ( flat_x != to.GridPos )
    {
      Vector2Int flat_y = from.GridPos + new Vector2Int( 0, rel_pos.y );
      check_positions.Add( new Vector3Int( flat_x.x, flat_x.y ) );
      check_positions.Add( new Vector3Int( flat_y.x, flat_y.y ) );
    }

    for (int i = 0; i < m_CollisionTilemaps.Count; i++)
    {
      foreach ( Vector3Int check in check_positions )
      {
        if ( m_CollisionTilemaps[i].GetColliderType( check ) == Tile.ColliderType.Grid )
        {
          if ( check.x == to.GridPos.x && check.y == to.GridPos.y )
          {
            return WalkableState.Closed;
          }
          return WalkableState.DiagonallyClosed;
        }
      }
    }
    return WalkableState.Open;
  }

  //-------------------------------------------------------------------------------------
  public bool HasObstruction( GridNode node )
  {
    for (int i = 0; i < m_CollisionTilemaps.Count; i++)
    {
      if ( m_CollisionTilemaps[i].GetColliderType( new Vector3Int( node.GridPos.x, node.GridPos.y ) ) == Tile.ColliderType.Grid )
      {
        return true;
      }
    }
    return false;
  }

  //-------------------------------------------------------------------------------------
  public void SetGridObject(int x, int y, GridNode value)
  {
    m_Nodes[new Vector2Int(x, y)] = value;
  }

  //-------------------------------------------------------------------------------------
  public GridNode GetGridObject(Vector2Int position)
  {
    if (m_Nodes.ContainsKey(position))
    {
      return m_Nodes[position];
    }
    InitGridObject( position );
    return m_Nodes[position];
  }
}
