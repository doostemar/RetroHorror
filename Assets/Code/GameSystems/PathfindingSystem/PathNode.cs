using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

  private Vector2Int m_Pos;
  public  Vector2Int GridPos { get { return m_Pos; } }

  private Vector2 m_WorldPos;
  public  Vector2 WorldPos { get { return m_WorldPos; } }
  
  public int      m_GCost;
  public int      m_HCost;
  public int      m_FCost;
  public PathNode m_PreviousNode;


  public PathNode( Vector2Int grid_pos, Vector2 world_pos )
  {
    m_Pos      = grid_pos;
    m_WorldPos = world_pos;
  }

  public Vector2Int GetGridPos()
  {
    return m_Pos;
  }

  public void CalculateFCost()
  {
    m_FCost = m_GCost + m_HCost;
  }
}
