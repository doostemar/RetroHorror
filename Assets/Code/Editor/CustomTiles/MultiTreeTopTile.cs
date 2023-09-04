using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiTreeTopTile : CustomTile
{
  public Sprite m_TopSprite;
  public Sprite m_ContinueToSingleSprite;

  public override void RefreshTile(Vector3Int pos, ITilemap tilemap)
  {
    Vector3Int up_pos  = new Vector3Int( pos.x, pos.y + 1, pos.z );
    CustomTile up_tile = tilemap.GetTile<CustomTile>( up_pos );
    
    if ( up_tile != null && up_tile.IsAnyType( Type.SingleTree ) )
    {
      tilemap.RefreshTile( up_pos );
    }

    Vector3Int down_pos = new Vector3Int(pos.x, pos.y - 1, pos.z);
    CustomTile down_tile = tilemap.GetTile<CustomTile>( down_pos );
    if ( down_tile != null && down_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) )
    {
      tilemap.RefreshTile( down_pos );
    }

    tilemap.RefreshTile( pos );
  }

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tile_data)
  {
    CustomTile up_tile = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0, 1, 0 ) );

    Sprite sprite = m_TopSprite;
    if ( up_tile != null && up_tile.IsType( Type.SingleTree ) )
    {
      sprite = m_ContinueToSingleSprite;
    }

    tile_data.color        = Color.white;
    tile_data.flags        = TileFlags.LockTransform;
    tile_data.colliderType = ColliderType.None;
    tile_data.sprite       = sprite;
  }

  #if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/MultiTreeTopTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save MultiTreeTopTile", "MultiTreeTopTile", "Asset", "Save MultiTreeTopTile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MultiTreeTopTile>(), path);
  }
#endif
}
