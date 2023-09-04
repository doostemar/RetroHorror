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

  public Matrix4x4 m_LowerLayerTransform = Matrix4x4.identity;

  public override void RefreshTile(Vector3Int pos, ITilemap tilemap)
  {
    // check up
    Vector3Int up_pos  = new Vector3Int( pos.x, pos.y + 1, pos.z );
    CustomTile up_tile = tilemap.GetTile<CustomTile>( up_pos );
    
    if ( up_tile != null && up_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) )
    {
      tilemap.RefreshTile( up_pos );
    }

    Vector3Int down_pos  = new Vector3Int(pos.x, pos.y - 1, pos.z);
    CustomTile down_tile = tilemap.GetTile<CustomTile>( down_pos );
    if ( down_tile != null && down_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) )
    {
      tilemap.RefreshTile( down_pos );
    }

    tilemap.RefreshTile( pos );
  }

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tile_data)
  {
    CustomTile up_tile   = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0,  1, 0 ) );
    CustomTile down_tile = tilemap.GetTile<CustomTile>( position + new Vector3Int( 0, -1, 0 ) );

    ColliderType collider_type  = ColliderType.None;
    Sprite       sprite         = m_BaseSprite;
    Matrix4x4    calc_transform = m_LowerLayerTransform;
    
    if ( up_tile != null )
    {
      if ( up_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) )
      {
        sprite = m_BaseSpriteContinue;
      }
    }

    if ( down_tile != null && down_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) )
    {
      calc_transform = transform;
      sprite = m_TopSprite;
      if ( up_tile != null )
      {
        if ( up_tile.IsAnyType( Type.SingleTree | Type.MultiTree ) && up_tile.IsType( Type.SingleTreeBase ) == false )
        {
          sprite = m_MidSprite;
        }
      }
    }
    else if ( down_tile == null || down_tile.IsType( Type.SingleTree ) == false )
    {
      collider_type = ColliderType.Grid;
    }

    tile_data.color        = color;
    tile_data.flags        = TileFlags.LockTransform | flags;
    tile_data.transform    = calc_transform;
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
