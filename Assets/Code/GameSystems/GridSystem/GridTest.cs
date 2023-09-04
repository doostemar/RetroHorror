using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{

  private Pathfinding pathfinding;
  private Grid<PathNode> grid;

    // Start is called before the first frame update
  private void Start()
  {
    // Grid grid = new Grid(20, 10);
    // grid = new Grid<bool>(4, 2, 1f, new Vector3(0, 0));

    pathfinding = new Pathfinding(10, 10);
    grid = pathfinding.GetGrid();
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector3 mouse_wp = GetMouseWorldPosition();
      pathfinding.GetGrid().GetXY(mouse_wp, out int x, out int y);
      Debug.Log("end: " + x + ", " + y);
      List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
      if (path != null)
      {
        for (int i = 0; i < path.Count - 1; i++)
        {
          Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) + Vector3.one * .5f, 
          new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) + Vector3.one * .5f, Color.green, 3f, false);
        }
      }
    }

    //  grid.SetGridObject(GetMouseWorldPosition(), true);
    //}
    if (Input.GetMouseButtonDown(1))
    {
      Vector3 mouse_wp = GetMouseWorldPosition();
      grid.GetXY(mouse_wp, out int x, out int y);
      PathNode node = grid.GetGridObject(x, y);
      if (node.m_IsWalkable)
      {
        node.SetWalkable(x, y, false);
        grid.SetGridObject(x, y, node);
      }
      else
      {
        node.SetWalkable(x, y, true);
        grid.SetGridObject(x, y, node);
      }
    }
  }

  public static Vector3 GetMouseWorldPosition()
  {
    Vector3 mouse_world_position = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    mouse_world_position.z = 0f;
    return mouse_world_position;
  }
  public static Vector3 GetMouseWorldPositionWithZ()
  {
    return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
  }
  public static Vector3 GetMouseWorldPositionWIthZ(Camera world_camera)
  {
    return GetMouseWorldPositionWithZ(Input.mousePosition, world_camera);
  }
  public static Vector3 GetMouseWorldPositionWithZ( Vector3 screen_position, Camera world_camera)
  {
    Vector3 world_position = world_camera.ScreenToWorldPoint(screen_position);
    return world_position;
  }
}
