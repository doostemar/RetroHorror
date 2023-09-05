using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CamsGrid<TGridObject>
{

  public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
  public class OnGridValueChangedEventArgs : EventArgs
  {
    public int x;
    public int y;
  }
  private int            m_Width;
  private int            m_Height;
  private float          m_CellSize;
  private Vector3        m_Origin;
  private TGridObject[,] m_GridArray;
  private TextMesh[,]    m_DebugGrid;

  public CamsGrid(int width, int height, float cell_size, Vector3 origin_position, Func<CamsGrid<TGridObject>, int, int, TGridObject> create_grid_object)
  {
    this.m_Width = width;
    this.m_Height = height;
    this.m_CellSize = cell_size;
    this.m_Origin = origin_position;

    m_GridArray = new TGridObject[m_Width, m_Height];
    for (int x = 0; x < m_GridArray.GetLength(0); x++)
    {
      for (int y = 0; y < m_GridArray.GetLength(1); y++)
      {
        m_GridArray[x, y] = create_grid_object(this, x, y);
      }
    }

    m_DebugGrid = new TextMesh[width, height];

    DrawDebugGrid();
    
  }

  public int GetWidth()
  {
    return m_Width;
  }

  public int GetHeight()
  {
    return m_Height;
  }

  private Vector3 GetWorldPosition(int x, int y)
  {
    return new Vector3(x, y) * m_CellSize + m_Origin;
  }

  public void GetXY(Vector3 world_position, out int x, out int y)
  {
    x = Mathf.FloorToInt((world_position - m_Origin).x / m_CellSize);
    y = Mathf.FloorToInt((world_position - m_Origin).y / m_CellSize);
  }

  public void SetGridObject(int x, int y, TGridObject value)
  {
    if (x >= 0 && y >= 0 && x < m_Width && y < m_Height)
    {
      m_GridArray[x, y] = value;
      m_DebugGrid[x, y].text = m_GridArray[x, y]?.ToString();
      TriggerGridObjectChanged(x, y);
    }
    else
    {
      Debug.LogError("Bad value or index out of bounds, you figure it out");
    }
  }

  public void TriggerGridObjectChanged(int x, int y)
  {
    if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
  }

  public void SetGridObject(Vector3 world_position, TGridObject value)
  {
    int x, y;
    GetXY(world_position, out x, out y);
    SetGridObject(x, y, value);
  }

  public TGridObject GetGridObject(int x, int y)
  {
    if (x >= 0 && y >= 0 && x < m_Width && y < m_Height)
    {
      return m_GridArray[x, y];
    }
    Debug.LogError("Value not found");
    return default(TGridObject);
  }

  public TGridObject GetGridObject(Vector3 world_position)
  {
    int x, y;
    GetXY(world_position, out x, out y);
    return GetGridObject(x, y);
  }

  private void DrawDebugGrid()
  {

    //Debug.Log("Width: " + width + " Height: " + height);

    for (int x = 0; x < m_GridArray.GetLength(0); x++)
    {
      for (int y = 0; y < m_GridArray.GetLength(1); y++)
      {
        //Debug.Log(x + ", " + y);
        // creates a grid of values
        m_DebugGrid[x, y] = CreateWorldText(m_GridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(m_CellSize, m_CellSize) * .5f, 40, Color.white, TextAnchor.MiddleCenter);
        // vertical line
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
        // horizontal line
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
      }
    }
    Debug.DrawLine(GetWorldPosition(0, m_Height), GetWorldPosition(m_Width, m_Height), Color.white, 100f);
    Debug.DrawLine(GetWorldPosition(m_Width, 0), GetWorldPosition(m_Width, m_Height), Color.white, 100f);

    //SetValue(2, 1, 56);
  }


  // Create Text in the World, should be in separate utilities class?
  public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
  {
    if (color == null) color = Color.white;
    return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
  }

  // Create Text in the World, should be in separate utilities class?
  public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
  {
    GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
    Transform transform = gameObject.transform;
    transform.SetParent(parent, false);
    transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    transform.localPosition = localPosition;
    TextMesh textMesh = gameObject.GetComponent<TextMesh>();
    textMesh.anchor = textAnchor;
    textMesh.alignment = textAlignment;
    textMesh.text = text;
    textMesh.fontSize = fontSize;
    textMesh.color = color;
    textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    return textMesh;
  }
}
