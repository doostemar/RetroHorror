using UnityEngine;
using UnityEngine.Tilemaps;


namespace UnityEditor.Tilemaps
{
  [CustomGridBrush(true, false, false, "MultiTree Brush")]
  public class MultiTreeBrush : GridBrush
  {
    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
      base.Paint(gridLayout, brushTarget, position);
    }

    [MenuItem("Assets/Create/CustomTilemapEditor/MultiTree Brush")]
    public static void CreateBrush()
    {
      string path = EditorUtility.SaveFilePanelInProject( "Save MultiTree Brush", "New MultiTree Brush", "Asset", "Save MultiTree Brush", "Assets/CustomTilemapEditor");
      if (path == "")
        return;
      AssetDatabase.CreateAsset( ScriptableObject.CreateInstance<MultiTreeBrush>(), path );
    }
  }

    [CustomEditor(typeof(MultiTreeBrush))]
    public class  MultiTreeBrushEditor : GridBrushEditor
    {
    public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
    {
      base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
    }
  }
}