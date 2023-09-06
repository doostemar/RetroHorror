using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Pathfinding
{
  private const int kStraightCost = 10;
  private const int kDiagonalCost = 14;

  private List<PathNode> m_OpenList;
  private List<PathNode> m_ClosedList;


  public List<PathNode> FindPath( Vector2Int start, Vector2Int end)
  {
    PathfindingGrid grid = new PathfindingGrid();

    PathNode start_node = grid.GetGridObject(new Vector2Int(start.x, start.y));
    PathNode end_node = grid.GetGridObject(new Vector2Int(end.x, end.y));

    m_OpenList = new List<PathNode> { start_node };
    m_ClosedList = new List<PathNode>();

    start_node.m_GCost = 0;
    start_node.m_HCost = CalculateDistanceCost(start_node, end_node);
    Debug.Log("hcost: " + start_node.m_HCost);
    start_node.CalculateFCost();

    while (m_OpenList.Count > 0)
    {
      PathNode current_node = GetLowestFCostNode(m_OpenList);
      if (current_node == end_node)
      {
        // reached the end
        return CalculatePath(end_node);
      }

      m_OpenList.Remove(current_node);
      m_ClosedList.Add(current_node);

      foreach (PathNode neighbor in GetNeighbors(grid, current_node))
      {
        if (m_ClosedList.Contains(neighbor)) continue;

        if (grid.IsWalkable(neighbor) == false)
        {
          m_ClosedList.Add(neighbor);
          continue;
        }

        int temp_gcost = current_node.m_GCost + CalculateDistanceCost(current_node, neighbor);
        if (temp_gcost < neighbor.m_GCost)
        {
          neighbor.m_PreviousNode = current_node;
          neighbor.m_GCost = temp_gcost;
          neighbor.m_HCost = CalculateDistanceCost(neighbor, end_node);
          neighbor.CalculateFCost();

          if (!m_OpenList.Contains(neighbor)) m_OpenList.Add(neighbor);
        }
      }
    }

    // open list is empty
    return null;
  }

  private List<PathNode> GetNeighbors(PathfindingGrid grid, PathNode current_node)
  {
    List<PathNode> neighbor_nodes = new List<PathNode>();

    for (int dx = -1; dx <= 1; ++dx)
    {
      for (int dy = -1; dy <= 1; ++dy)
      {
        Vector2Int node_pos = current_node.GetPos() + new Vector2Int(dx, dy);
        PathNode node = grid.GetGridObject(node_pos);
        if (node != null)
        {
          neighbor_nodes.Add(node);
        }
      }
    }
    return neighbor_nodes;
  }

    private List<PathNode> CalculatePath(PathNode end_node)
  {
    List<PathNode> path = new List<PathNode>();
    path.Add(end_node);
    PathNode current_node = end_node;
    while (current_node.m_PreviousNode != null)
    { 
      path.Add(current_node.m_PreviousNode);
      current_node = current_node.m_PreviousNode;
    }
    path.Reverse();
    Debug.Log("nodes in path: " + path.Count);
    return path;
  }

  private int CalculateDistanceCost(PathNode start, PathNode end)
  {
    int x_distance = Mathf.Abs(start.GetPos().x - end.GetPos().x);
    int y_distance = Mathf.Abs(start.GetPos().y - end.GetPos().y);
    int remaining = Mathf.Abs(x_distance - y_distance);

    return kDiagonalCost * Mathf.Min(x_distance, y_distance) + kStraightCost * remaining;
  }

  private PathNode GetLowestFCostNode(List<PathNode> nodes)
  {
    PathNode lowest_fcost_node = nodes[0];
    for (int i = 1; i < nodes.Count; i++)
    { 
      if (nodes[i].m_FCost < lowest_fcost_node.m_FCost) lowest_fcost_node = nodes[i];
    }

    return lowest_fcost_node;
  }

}