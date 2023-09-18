using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
  private Vector2Int m_Pos;
  public  Vector2Int GridPos { get { return m_Pos; } }
  
  public int      m_GCost;
  public int      m_HCost;
  public int      m_FCost;
  public GridNode m_PreviousNode;

  public GridNode( Vector2Int grid_pos )
  {
    m_Pos      = grid_pos;
  }

  public void CalculateFCost()
  {
    m_FCost = m_GCost + m_HCost;
  }
}
