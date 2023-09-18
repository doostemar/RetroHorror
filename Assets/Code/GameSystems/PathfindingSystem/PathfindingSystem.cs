//#define PATHFINDING_LOGGING

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingSystem : MonoBehaviour
{
  private const int kStraightCost = 10;
  private const int kDiagonalCost = 14;

  private List<Tilemap> m_CollisionTilemaps;
  private BoundsInt     m_GridBounds;

  //-------------------------------------------------------------------------------------
  private void Start()
  {
    m_GridBounds = new BoundsInt();

    GameObject[] tilemap_objects = GameObject.FindGameObjectsWithTag("CollisionGrid");
    m_CollisionTilemaps = new List<Tilemap>();
    foreach (var tilemap_object in tilemap_objects)
    {
      Tilemap tm = tilemap_object.GetComponent<Tilemap>();
      if (tm != null)
      {
        tm.CompressBounds();
        m_CollisionTilemaps.Add( tm );

        BoundsInt tilemap_bounds = tm.cellBounds;
        m_GridBounds = MathHelper.GrowBounds( m_GridBounds, tilemap_bounds );
      }
    }
  }

  //-------------------------------------------------------------------------------------
  public Vector2 GetTileSize()
  {
    return m_CollisionTilemaps[0].cellSize;
  }

  //-------------------------------------------------------------------------------------
  public Vector2 GetGridWorldPosition( Vector2Int grid_pos )
  {
    return m_CollisionTilemaps[0].CellToWorld( new Vector3Int( grid_pos.x, grid_pos.y ) );
  }

  //-------------------------------------------------------------------------------------
  public List<PathNode> FindPath( Vector2 start, Vector2 end )
  {
    PathfindingGrid grid = new PathfindingGrid( m_CollisionTilemaps );
    Vector2Int grid_start = grid.WorldToGrid( start );
    Vector2Int grid_end   = grid.WorldToGrid( end );

    GridNode start_node = grid.GetGridObject( grid_start );
    GridNode end_node   = FindGoodEndNode( grid, start_node, grid_end );
    if ( end_node == null )
    {
      return CalculatePath( grid, start_node, start, end );
    }

    HashSet<GridNode> open_list   = new HashSet<GridNode> { start_node };
    HashSet<GridNode> closed_list = new HashSet<GridNode>();

    start_node.m_GCost = 0;
    start_node.m_HCost = CalculateDistanceCost(start_node, end_node);

    #if PATHFINDING_LOGGING
    Debug.Log("hcost: " + start_node.m_HCost);
    #endif

    start_node.CalculateFCost();

    while ( open_list.Count > 0 )
    {
      GridNode current_node = GetLowestFCostNode( open_list );
      if (current_node == end_node)
      {
        // reached the end
        return CalculatePath( grid, end_node, start, end );
      }

      open_list.Remove(current_node);
      closed_list.Add(current_node);

      foreach (GridNode neighbor in GetNeighbors( grid, current_node ) )
      {
        if (closed_list.Contains( neighbor ) ) continue;

        PathfindingGrid.WalkableState walkable_state = grid.FindWalkableState( current_node, neighbor );
        if ( walkable_state != PathfindingGrid.WalkableState.Open )
        {
          if ( walkable_state == PathfindingGrid.WalkableState.Closed )
          {
            closed_list.Add( neighbor );
          }
          continue;
        }

        int temp_gcost = current_node.m_GCost + CalculateDistanceCost(current_node, neighbor);
        if (temp_gcost < neighbor.m_GCost)
        {
          neighbor.m_PreviousNode = current_node;
          neighbor.m_GCost = temp_gcost;
          neighbor.m_HCost = CalculateDistanceCost(neighbor, end_node);
          neighbor.CalculateFCost();

          if (!open_list.Contains(neighbor)) open_list.Add(neighbor);
        }
      }
    }

    // open list is empty
    return null;
  }

  //-------------------------------------------------------------------------------------
  private GridNode FindGoodEndNode( PathfindingGrid grid, GridNode start_node, Vector2Int end_position )
  {
    GridNode end = grid.GetGridObject( end_position );

    while ( grid.HasObstruction( end ) )
    {
      Vector2Int dir_to_start = start_node.GridPos - end.GridPos;
      dir_to_start.Clamp( -Vector2Int.one, Vector2Int.one );
      end = grid.GetGridObject( end.GridPos + dir_to_start );

      if ( start_node == end )
      {
        return null;
      }
    }

    return end;
  }

  //-------------------------------------------------------------------------------------
  private List<GridNode> GetNeighbors(PathfindingGrid grid, GridNode current_node)
  {
    List<GridNode> neighbor_nodes = new List<GridNode>();

    for (int dx = -1; dx <= 1; ++dx)
    {
      for (int dy = -1; dy <= 1; ++dy)
      {
        Vector2Int node_pos = current_node.GridPos + new Vector2Int(dx, dy);
        
        if ( m_GridBounds.Contains( new Vector3Int( node_pos.x, node_pos.y ) ) )
        {
          GridNode node = grid.GetGridObject( node_pos );
          if (node != null)
          {
            neighbor_nodes.Add(node);
          }
        }
      }
    }
    return neighbor_nodes;
  }

  //-------------------------------------------------------------------------------------
  private List<PathNode> CalculatePath( PathfindingGrid grid, GridNode end_node, Vector2 start_pos, Vector2 end_pos )
  {
    List< GridNode > grid_list = new List< GridNode >();
    for ( GridNode node = end_node; node != null; node = node.m_PreviousNode )
    {
      grid_list.Add( node ) ;
    }

    grid_list.Reverse();
    List<PathNode> path = new List<PathNode>();
    for ( int i_node = 0; i_node < grid_list.Count; ++i_node )
    {
      Vector2 prev_pos = ( i_node == 0 ) ? start_pos : path[ i_node - 1 ].WorldPosition;
      PathNode pn = new PathNode( grid_list[ i_node ].GridPos, prev_pos, m_CollisionTilemaps[0] );
      path.Add( pn );
    }


    // Either walk directly to the targeted position (if it's reachable)
    PathNode last_node = path[ path.Count - 1 ];

    Vector2Int end_pos_tile = grid.WorldToGrid( end_pos );
    if ( last_node.GridPosition == end_pos_tile )
    {
      PathNode final_node = new PathNode( end_pos_tile, end_pos );
      path.Add( final_node );
    }
    else // or get as close as you can in the tile that was selected
    {
      Vector2Int final_grid      = last_node.GridPosition;
      Vector3    grid_world_pos  = m_CollisionTilemaps[ 0 ].CellToWorld( new Vector3Int( final_grid.x, final_grid.y ) );

      Vector3 cell_size      =  m_CollisionTilemaps[ 0 ].cellSize;
      Vector3 cell_extents   = cell_size / 2f;
      Vector3 bounds_extents = cell_size * 2f / 3f; // restrict the size of the final bounds so that we don't accidentally spill over into an adjacent tile

      Bounds cell_bounds = new Bounds( grid_world_pos + cell_extents, bounds_extents );
      Vector3 world_pos  = cell_bounds.ClosestPoint( end_pos );

      PathNode final_node = new PathNode( final_grid, world_pos );
      path.Add( final_node );
    }

    #if PATHFINDING_LOGGING
    Debug.Log("nodes in path: " + path.Count);
    #endif
    
    return path;
  }

  //-------------------------------------------------------------------------------------
  private int CalculateDistanceCost(GridNode start, GridNode end)
  {
    int x_distance = Mathf.Abs(start.GridPos.x - end.GridPos.x);
    int y_distance = Mathf.Abs(start.GridPos.y - end.GridPos.y);
    int remaining  = Mathf.Abs(x_distance - y_distance);

    return kDiagonalCost * Mathf.Min(x_distance, y_distance) + kStraightCost * remaining;
  }

  //-------------------------------------------------------------------------------------
  private GridNode GetLowestFCostNode( HashSet<GridNode> nodes )
  {
    GridNode lowest_cost = null;
    foreach( GridNode node in nodes )
    {
      if ( lowest_cost == null 
        || node.m_FCost < lowest_cost.m_FCost )
      {
        lowest_cost = node;
      }
    }

    return lowest_cost;
  }

}