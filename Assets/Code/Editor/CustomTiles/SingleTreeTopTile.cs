using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SingleTreeTopTile : CustomTile
{
  public Sprite m_SpriteSolo;
  public Sprite m_SpriteEnd;
  public Sprite m_SpriteContinue;

  public override void RefreshTile( Vector3Int pos, ITilemap tilemap )
  {
    Vector3Int up_pos = new Vector3Int( pos.x, pos.y + 1, pos.z );
    Type up_type = GetTileType( tilemap, up_pos );
    if ( ( up_type & Type.SingleTree ) == Type.SingleTree )
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
    Type up_type   = GetTileType( tilemap, position + new Vector3Int( 0,  1, 0 ) );
    Type down_type = GetTileType( tilemap, position + new Vector3Int( 0, -1, 0 ) );

    Sprite       sprite   = m_SpriteSolo;
    ColliderType collider = ColliderType.Grid;
    if ( ( up_type & Type.SingleTree )     == Type.SingleTree &&
         ( up_type & Type.SingleTreeBase ) == 0 
       )
    {
      sprite = m_SpriteContinue;
      collider = ColliderType.None;
    }
    else if ( up_type == 0 || ( up_type & Type.SingleTreeBase ) == Type.SingleTreeBase )
    {
      if ( ( down_type & Type.SingleTree ) == Type.SingleTree)
      {
        sprite = m_SpriteEnd;
        collider = ColliderType.None;
      }
    }

    tile_data.color        = Color.white;
    tile_data.flags        = TileFlags.LockTransform;
    tile_data.colliderType = collider;
    tile_data.sprite       = sprite;
  }

#if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/SingleTreeTopCustomTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save Single Tree Top Tile", "New Single Tree Top Tile", "Asset", "Save Single Tree Top Tile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SingleTreeTopTile>(), path);
  }
#endif
}
