using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingGridTest : MonoBehaviour
{

  public Tilemap map;
  private PathfindingSystem pathfinding;

    // Start is called before the first frame update
  private void Start()
  {

    pathfinding = new PathfindingSystem();
    //grid = pathfinding.GetGrid();
  }

  private void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector3 mouse_wp = GetMouseWorldPosition();
      Vector3Int dest = map.WorldToCell(mouse_wp);
      Vector3 pos = GameObject.FindGameObjectWithTag("Hero").transform.position;
      Vector2Int origin = new Vector2Int((int)pos.x, (int)pos.y);
      List<PathNode> path = pathfinding.FindPath(origin, new Vector2Int(dest.x, dest.y));
      if (path != null)
      {
        for (int i = 0; i < path.Count - 1; i++)
        {
          Debug.DrawLine(new Vector3(path[i].GetPos().x, path[i].GetPos().y) + Vector3.one * .5f,
          new Vector3(path[i + 1].GetPos().x, path[i + 1].GetPos().y) + Vector3.one * .5f, Color.green, 3f, false);
        }
      }
    }
    //  //  grid.SetGridObject(GetMouseWorldPosition(), true);
    //  //}
    //  if (Input.GetMouseButtonDown(1))
    //  {
    //    Vector3 mouse_wp = GetMouseWorldPosition();
    //    grid.GetXY(mouse_wp, out int x, out int y);
    //    PathNode node = grid.GetGridObject(x, y);
    //    if (node.m_IsWalkable)
    //    {
    //      node.SetWalkable(false);
    //      grid.SetGridObject(x, y, node);
    //    }
    //    else
    //    {
    //      node.SetWalkable(true);
    //      grid.SetGridObject(x, y, node);
    //    }
    //  }
    //
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
