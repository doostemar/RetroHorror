using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
  private Grid<PathNode>  grid;
  private int             m_XPos;
  private int             m_YPos;
  
  public int      m_GCost;
  public int      m_HCost;
  public int      m_FCost;
  public PathNode m_PreviousNode;

  public bool m_IsWalkable;

  public PathNode(Grid<PathNode> grid, int x_pos, int y_pos)
  {
    this.grid = grid;
    m_XPos = x_pos;
    m_YPos = y_pos;
    m_IsWalkable = true;
  }

  public void SetWalkable(int x_pos, int y_pos, bool walkable)
  {
    m_IsWalkable = walkable;
  }

  public int GetX() { return m_XPos; }

  public int GetY() { return m_YPos; }

  public void CalculateFCost()
  {
    m_FCost = m_GCost + m_HCost;
  }

  public override string ToString()
  {
    if (!m_IsWalkable)
    {
      return "NO!";
    }
    return m_XPos + ", " + m_YPos;
  }
}
