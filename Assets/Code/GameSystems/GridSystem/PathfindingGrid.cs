using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class PathfindingGrid
{

  //public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
  //public class OnGridValueChangedEventArgs : EventArgs
  //{
  //  public int x;
  //  public int y;
  //}

  private Dictionary<Vector2Int, PathNode> m_Nodes;
  private List<Tilemap> m_CollisionTilemaps; 
  private TextMesh[,]    m_DebugGrid;

  public PathfindingGrid()
  {
    m_Nodes = new Dictionary<Vector2Int, PathNode>();

    // m_DebugGrid = new TextMesh[m_Width, m_Height];

    GameObject[] tilemap_objects = GameObject.FindGameObjectsWithTag("CollisionGrid");
    m_CollisionTilemaps = new List<Tilemap>();
    foreach (var tilemap_object in tilemap_objects)
    {
      Tilemap tm = tilemap_object.GetComponent<Tilemap>();
      if (tm != null)
      {
        m_CollisionTilemaps.Add(tm);
      }
    }

    // DrawDebugGrid();
    
  }

  public void InitGridObject(Vector2Int position)
  {
    PathNode node = new PathNode(position);
    node.m_GCost = int.MaxValue;
    node.CalculateFCost();
    node.m_PreviousNode = null;
    m_Nodes[position] = node;
  }

  public bool IsWalkable(PathNode node)
  {
    for (int i = 0; i < m_CollisionTilemaps.Count; i++)
    {
      if (m_CollisionTilemaps[i].GetColliderType(new Vector3Int(node.GetPos().x, node.GetPos().y))  == Tile.ColliderType.Grid)
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

  //public void TriggerGridObjectChanged(int x, int y)
  //{
  //  if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
  //}

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
