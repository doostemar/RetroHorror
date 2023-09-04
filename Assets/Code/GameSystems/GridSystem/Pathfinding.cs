using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

  private const int kStraightCost = 10;
  private const int kDiagonalCost = 14;

  private Grid<PathNode> m_Grid;
  private List<PathNode> m_OpenList;
  private List<PathNode> m_ClosedList;

  public Pathfinding(int width, int height)
  {
    m_Grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (Grid<PathNode> node, int x_pos, int y_pos) => new PathNode(node, x_pos, y_pos));
  }

  public Grid<PathNode> GetGrid()
  {
    return m_Grid;
  }

  public List<PathNode> FindPath( int x_start, int y_start, int x_end, int y_end)
  {
    PathNode start_node = m_Grid.GetGridObject(x_start, y_start);
    PathNode end_node = m_Grid.GetGridObject(x_end, y_end);

    m_OpenList = new List<PathNode> { start_node };
    m_ClosedList = new List<PathNode>();

    for (int x = 0; x < m_Grid.GetWidth(); x++)
    {
      for (int y = 0; y < m_Grid.GetHeight(); y++)
      {
        PathNode node = m_Grid.GetGridObject(x, y);
        node.m_GCost = int.MaxValue;
        node.CalculateFCost();
        node.m_PreviousNode = null;
      }
    }

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

      foreach (PathNode neighbor in GetNeighbors(current_node))
      {
        if (m_ClosedList.Contains(neighbor)) continue;

        if (!neighbor.m_IsWalkable)
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

  private List<PathNode> GetNeighbors(PathNode current_node)
  {
    List<PathNode> neighbor_nodes = new List<PathNode>();

    if (current_node.GetX() - 1 >= 0) 
    { // left neighbor
      neighbor_nodes.Add(GetNode(current_node.GetX() - 1, current_node.GetY()));
      // bottom left
      if (current_node.GetY() - 1 >= 0) neighbor_nodes.Add(GetNode(current_node.GetX() - 1, current_node.GetY() - 1));
      // upper left
      if (current_node.GetY() + 1 < m_Grid.GetHeight()) neighbor_nodes.Add(GetNode(current_node.GetX() - 1, current_node.GetY() + 1));
    }
    if (current_node.GetX() + 1 < m_Grid.GetWidth())
    { // right neighbor
      neighbor_nodes.Add(GetNode(current_node.GetX() + 1, current_node.GetY()));
      // bottom right
      if (current_node.GetY() - 1 >= 0) neighbor_nodes.Add(GetNode(current_node.GetX() + 1, current_node.GetY() - 1));
      // upper right
      if (current_node.GetY() + 1 < m_Grid.GetHeight()) neighbor_nodes.Add(GetNode(current_node.GetX() + 1, current_node.GetY() + 1));
    }
    // bottom
    if (current_node.GetY() - 1 >= 0) neighbor_nodes.Add(GetNode(current_node.GetX(), current_node.GetY() - 1));
    // up
    if (current_node.GetY() + 1 < m_Grid.GetHeight()) neighbor_nodes.Add(GetNode(current_node.GetX(), current_node.GetY() + 1));

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

  private PathNode GetNode(int x_pos, int y_pos)
  {
    return m_Grid.GetGridObject(x_pos, y_pos);
  }

  private int CalculateDistanceCost(PathNode start, PathNode end)
  {
    int x_distance = Mathf.Abs(start.GetX() - end.GetX());
    int y_distance = Mathf.Abs(start.GetY() - end.GetY());
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
