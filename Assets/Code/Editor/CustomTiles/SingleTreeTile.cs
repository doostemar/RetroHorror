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
    Vector3Int up_pos = new Vector3Int( pos.x, pos.y + 1, pos.z );
    
    if ( ( GetTileType( tilemap, up_pos ) & Type.SingleTree ) == Type.SingleTree )
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

    ColliderType collider_type = ColliderType.None;
    Sprite       sprite        = ( up_type == Type.SingleTree ) ? m_BaseSpriteContinue : m_BaseSprite;

    if ( ( down_type & Type.SingleTree ) == Type.SingleTree )
    {
      sprite = ( up_type == 0 || ( up_type & Type.SingleTreeBase ) == Type.SingleTreeBase ) ? m_TopSprite : m_MidSprite;
    }
    else if ( down_type == 0 )
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
