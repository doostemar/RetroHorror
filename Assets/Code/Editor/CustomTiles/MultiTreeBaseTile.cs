using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiTreeBaseTile : CustomTile
{
  public Sprite m_ContinueSprite;
  public Sprite m_SoloSprite;
  public Sprite m_ContinueFromSingleSprite;

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tile_data)
  {
    CustomTile up_tile   = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0, 1, 0 ) );
    CustomTile down_tile = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0, -1, 0 ) );

    Sprite sprite = m_SoloSprite;
    if ( up_tile != null && up_tile.IsType( Type.MultiTree ) )
    {
      sprite = m_ContinueSprite;
      if ( down_tile != null && down_tile.IsType( Type.SingleTree ) )
      {
        sprite = m_ContinueFromSingleSprite;
      }
    }

    tile_data.color = Color.white;
    tile_data.flags = TileFlags.LockTransform;
    tile_data.colliderType = ColliderType.Grid;
    tile_data.sprite = sprite;
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
