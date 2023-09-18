using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathNode
{
  private Vector2Int m_GridPosition;
  public  Vector2Int GridPosition { get { return m_GridPosition; } }

  private Vector2    m_WorldPosition;
  public  Vector2    WorldPosition {  get { return m_WorldPosition; } }

  public PathNode( Vector2Int grid_pos, Vector2 prev_world_pos, Tilemap tm )
  {
    m_GridPosition = grid_pos;
    Vector3 ll_corner    = tm.CellToWorld( new Vector3Int( grid_pos.x, grid_pos.y ) );
    Vector3 cell_extents = tm.cellSize / 2f;

    Bounds cell_bounds = new Bounds( ll_corner + cell_extents, tm.cellSize );
    m_WorldPosition = cell_bounds.ClosestPoint( prev_world_pos );
  }

  public PathNode( Vector2Int grid_pos, Vector2 abs_pos )
  {
    m_GridPosition  = grid_pos;
    m_WorldPosition = abs_pos;
  }
}
