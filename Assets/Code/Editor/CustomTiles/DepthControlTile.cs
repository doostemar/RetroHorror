using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DepthControlTile : Tile
{
  public float m_Depth;

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tile_data)
  {
    Matrix4x4 transform_copy = transform;
    transform_copy.m13       = m_Depth;

    tile_data.color        = color;
    tile_data.flags        = flags;
    tile_data.transform    = transform_copy;
    tile_data.colliderType = colliderType;
    tile_data.sprite       = sprite;
  }

  #if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/DepthTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject(
      "Save Depth Tile",
      "New Depth Tile",
      "Asset",
      "Save Depth Tile",
      "Assets/CustomTilemapEditor"
    );
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DepthControlTile>(), path);
  }
  #endif
}
