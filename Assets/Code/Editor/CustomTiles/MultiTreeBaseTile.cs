using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiTreeBaseTile : CustomTile
{
  public Sprite m_ContinueSprite;
  public Sprite m_SoloSprite;

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tile_data)
  {
    Type up_type = GetTileType( tilemap, position + new Vector3Int( 0, 1, 0 ) );

    tile_data.color = Color.white;
    tile_data.flags = TileFlags.LockTransform;
    tile_data.colliderType = ColliderType.Grid;
    tile_data.sprite = ( up_type & Type.MultiTree ) == Type.MultiTree ? m_ContinueSprite : m_SoloSprite;
  }

#if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/MultiTreeBaseTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save Multi Tree Base Tile", "New Multi Tree Base Tile", "Asset", "Save Multi Tree Base Tile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MultiTreeBaseTile>(), path);
  }
#endif
}
