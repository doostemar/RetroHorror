using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiTreePeripheryTile : CustomTile
{

#if UNITY_EDITOR
  [MenuItem("Assets/Create/CustomTilemapEditor/MultiTreePeripheryTile")]
  public static void CreateSingleTreeTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save Multi Periphery Tile", "MultiTreePeriphery", "Asset", "Save Multi Tree Periphery Tile", "Assets/CustomTilemapEditor");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MultiTreePeripheryTile>(), path);
  }
#endif
}
