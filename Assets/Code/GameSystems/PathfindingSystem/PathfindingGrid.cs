using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static UnityEngine.Rendering.DebugUI;

public class PathfindingGrid
{
  private Dictionary<Vector2Int, PathNode> m_Nodes;
  private List<Tilemap> m_CollisionTilemaps; 
  // private TextMesh[,]   m_DebugGrid;

  public PathfindingGrid( List<Tilemap> collision_tilemaps )
  {
    m_Nodes = new Dictionary<Vector2Int, PathNode>();
    m_CollisionTilemaps = collision_tilemaps;
  }

  // I think I overcomplicated this. Should probably be re-evaluated
  public Vector2Int WorldToGrid( Vector2 pos )
  {
    if ( m_CollisionTilemaps.Count > 0 )
    {
      Vector3Int ll_cell_pos = m_CollisionTilemaps[0].WorldToCell(pos);
      Vector3Int[] all_corners =
      {
        ll_cell_pos,
        ll_cell_pos + new Vector3Int( 0, 1 ),
        ll_cell_pos + new Vector3Int( 1, 0 ),
        ll_cell_pos + new Vector3Int( 1, 1 ),
      };

      float min_dist = float.MaxValue;
      int   min_idx  = int.MaxValue;

      int idx = 0;
      foreach ( Vector3Int corner in all_corners )
      {
        Vector2 world = m_CollisionTilemaps[0].CellToWorld( corner );
        float dist = (world - pos).sqrMagnitude;
        if ( dist < min_dist )
        {
          min_dist = dist;
          min_idx =  idx;
        }
        idx++;
      }


      return new Vector2Int( all_corners[ min_idx ].x, all_corners[ min_idx ].y );
    }
    return Vector2Int.zero;
  }

  public void InitGridObject( Vector2Int position )
  {
    Vector2 world_pos = Vector2.zero;
    if ( m_CollisionTilemaps.Count > 0 )
    {
      world_pos = m_CollisionTilemaps[0].CellToWorld( new Vector3Int( position.x, position.y ) );
    }
    else
    {
      Debug.LogError( "No CollisionGrid tags in scene. Pathfinding will not work correctly" );
    }

    PathNode node = new PathNode( position, world_pos );
    node.m_GCost = int.MaxValue;
    node.CalculateFCost();
    node.m_PreviousNode = null;
    m_Nodes[position] = node;

  }

  public bool IsWalkable(PathNode node)
  {
    for (int i = 0; i < m_CollisionTilemaps.Count; i++)
    {
      if (m_CollisionTilemaps[i].GetColliderType(new Vector3Int(node.GridPos.x, node.GridPos.y))  == Tile.ColliderType.Grid)
      {
        return false;
      }
    }
    return true;
  }

  public void SetGridObject(int x, int y, PathNode value)
  {
    m_Nodes[new Vector2Int(x, y)] = value;
  }

  public PathNode GetGridObject(Vector2Int position)
  {
    if (m_Nodes.ContainsKey(position))
    {
      return m_Nodes[position];
    }
    InitGridObject(position);
    return m_Nodes[position];
  }

  //private void DrawDebugGrid()
  //{

  //  //Debug.Log("Width: " + width + " Height: " + height);

  //  for (int x = 0; x < m_Width; x++)
  //  {
  //    for (int y = 0; y < m_Height; y++)
  //    {
  //      //Debug.Log(x + ", " + y);
  //      // creates a grid of values

  //      CreateWorldText(m_Nodes.GetValueOrDefault<Vector2Int, PathNode>(new Vector2Int(x, y))?.ToString(), null, GetWorldPosition(x, y) + new Vector3(m_CellSize, m_CellSize) * .5f, 40, Color.white, TextAnchor.MiddleCenter);
  //      // vertical line
  //      Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
  //      // horizontal line
  //      Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
  //    }
  //  }
  //  Debug.DrawLine(GetWorldPosition(0, m_Height), GetWorldPosition(m_Width, m_Height), Color.white, 100f);
  //  Debug.DrawLine(GetWorldPosition(m_Width, 0), GetWorldPosition(m_Width, m_Height), Color.white, 100f);

  //  //SetValue(2, 1, 56);
  //}


  //// Create Text in the World, should be in separate utilities class?
  //public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
  //{
  //  if (color == null) color = Color.white;
  //  return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
  //}

  //// Create Text in the World, should be in separate utilities class?
  //public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
  //{
  //  GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
  //  Transform transform = gameObject.transform;
  //  transform.SetParent(parent, false);
  //  transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
  //  transform.localPosition = localPosition;
  //  TextMesh textMesh = gameObject.GetComponent<TextMesh>();
  //  textMesh.anchor = textAnchor;
  //  textMesh.alignment = textAlignment;
  //  textMesh.text = text;
  //  textMesh.fontSize = fontSize;
  //  textMesh.color = color;
  //  textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
  //  return textMesh;
  //}
}
