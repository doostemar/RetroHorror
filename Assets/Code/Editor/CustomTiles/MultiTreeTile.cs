using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiTreeTile : CustomTile
{
  public override void RefreshTile( Vector3Int position, ITilemap tilemap )
  {
    Vector3Int down_pos = position + new Vector3Int( 0, -1, 0 );
    Type down_type = GetTileType( tilemap, down_pos );

    if ( ( down_type & Type.MultiTreeBase ) != 0 )
    {
      tilemap.RefreshTile( down_pos );
    }

    base.RefreshTile(position, tilemap);
  }

  public override void GetTileData( Vector3Int position, ITilemap tilemap, ref TileData tile_data )
  {
    tile_data.color        = color;
    tile_data.flags        = flags;
    tile_data.colliderType = colliderType;
    tile_data.transform    = transform;
    tile_data.sprite       = sprite;
  }

#if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/MultiTreeTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save Multi Tree Tile", "New Multi Tree Tile", "Asset", "Save Multi Tree Tile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MultiTreeTile>(), path);
  }
#endif
}
