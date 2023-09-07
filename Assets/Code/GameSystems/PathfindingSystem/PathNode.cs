using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

  private Vector2Int m_Pos;
  
  public int      m_GCost;
  public int      m_HCost;
  public int      m_FCost;
  public PathNode m_PreviousNode;


  public PathNode(Vector2Int position)
  {
    m_Pos = position;
  }

  public Vector2Int GetPos()
  {
    return m_Pos;
  }

  public void CalculateFCost()
  {
    m_FCost = m_GCost + m_HCost;
  }

  //public override string ToString()
  //{
  //  if (!m_IsWalkable)
  //  {
  //    return "NO!";
  //  }
  //  return m_XPos + ", " + m_YPos;
  //}
}
