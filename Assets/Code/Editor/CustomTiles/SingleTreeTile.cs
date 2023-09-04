using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SingleTreeTile : CustomTile
{
  public Sprite m_TopSprite;
  public Sprite m_BaseSpriteContinue;
  public Sprite m_BaseSprite;
  public Sprite m_MidSprite;

  public override void RefreshTile(Vector3Int pos, ITilemap tilemap)
  {
    // check up
    Vector3Int up_pos  = new Vector3Int( pos.x, pos.y + 1, pos.z );
    CustomTile up_tile = tilemap.GetTile<CustomTile>( up_pos );
    
    if ( up_tile != null && up_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) ) //( GetTileType( tilemap, up_pos ) & Type.SingleTree ) == Type.SingleTree )
    {
      tilemap.RefreshTile( up_pos );
    }

    Vector3Int down_pos = new Vector3Int(pos.x, pos.y - 1, pos.z);
    Type down_type = GetTileType( tilemap, down_pos );
    if ( ( down_type & Type.SingleTree ) == Type.SingleTree )
    {
      tilemap.RefreshTile( down_pos );
    }

    tilemap.RefreshTile( pos );
  }

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tile_data)
  {
    CustomTile up_tile   = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0,  1, 0 ) );
    CustomTile down_tile = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0, -1, 0 ) );

    ColliderType collider_type = ColliderType.None;
    Sprite       sprite        = up_tile != null && up_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) ? m_BaseSpriteContinue : m_BaseSprite;

    if ( down_tile != null && down_tile.IsType( Type.SingleTree ) )
    {
      sprite = ( up_tile == null || up_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) == false  || up_tile.IsType( Type.SingleTreeBase ) ) ? m_TopSprite : m_MidSprite;
    }
    else if ( down_tile == null || down_tile.IsType( Type.SingleTree ) == false )
    {
      collider_type = ColliderType.Grid;
    }

    tile_data.color        = Color.white;
    tile_data.flags        = TileFlags.LockTransform;
    tile_data.colliderType = collider_type;
    tile_data.sprite       = sprite;
  }

#if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/SingleTreeCustomTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save Single Tree Tile", "New Single Tree Tile", "Asset", "Save Single Tree Tile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SingleTreeTile>(), path);
  }
#endif
}
