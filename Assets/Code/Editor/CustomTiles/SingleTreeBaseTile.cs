using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SingleTreeBaseTile : CustomTile
{
  public Sprite m_SpriteContinue;
  public Sprite m_SingleSprite;

  public override void RefreshTile(Vector3Int pos, ITilemap tilemap)
  {
    // check up
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

    tile_data.color        = color;
    tile_data.flags        = flags;
    tile_data.transform    = transform;
    tile_data.colliderType = ColliderType.Grid;
    tile_data.sprite       = ( up_type & Type.SingleTree ) == Type.SingleTree ? m_SpriteContinue : m_SingleSprite;
  }

#if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/SingleTreeBaseCustomTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save Single Tree Base Tile", "New Single Tree Base Tile", "Asset", "Save Single Tree Base Tile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SingleTreeBaseTile>(), path);
  }
#endif
}
