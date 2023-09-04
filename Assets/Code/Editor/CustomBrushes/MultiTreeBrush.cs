using UnityEngine;
using UnityEngine.Tilemaps;


namespace UnityEditor.Tilemaps
{
  [CustomGridBrush(true, false, false, "MultiTree Brush")]
  public class MultiTreeBrush : GridBrush
  {
    public CustomTile m_MidTile;
    public CustomTile m_BaseTile;
    public CustomTile m_TopTile;
    public CustomTile m_BaseLeftPeripheryTile;
    public CustomTile m_BaseRightPeripheryTile;
    public CustomTile m_BaseComboPeripheryTile;
    public CustomTile m_TopLeftPeripheryTile;
    public CustomTile m_TopRightPerirpheryTile;
    public CustomTile m_TopComboPeripheryTile;
    public CustomTile m_SingleComboBaseLeftPeripheryTile;
    public CustomTile m_SingleComboBaseRightPeripheryTile;
    public CustomTile m_SingleComboBaseComboPeripheryTile;
    public CustomTile m_SingleBaseComboBaseLeftPeripheryTile;
    public CustomTile m_SingleBaseComboBaseRightPeripheryTile;
    public CustomTile m_SingleBaseComboBaseComboPeripheryTile;
    public CustomTile m_SingleTopComboBaseLeftPeripheryTile;
    public CustomTile m_SingleTopComboBaseRightPeripheryTile;
    public CustomTile m_SingleTopComboBaseComboPeripheryTile;
    public CustomTile m_SingleComboTopLeftPeripheryTile;
    public CustomTile m_SingleComboTopRightPeripheryTile;
    public CustomTile m_SingleComboTopComboPeripheryTile;
    public CustomTile m_SingleBaseComboTopLeftPeripheryTile;
    public CustomTile m_SingleBaseComboTopRightPeripheryTile;
    public CustomTile m_SingleBaseComboTopComboPeripheryTile;
    public CustomTile m_SingleTopComboTopLeftPeripheryTile;
    public CustomTile m_SingleTopComboTopRightPeripheryTile;
    public CustomTile m_SingleTopComboTopComboPeripheryTile;

    public SingleTreeTile     m_SingleTreeTile;
    public SingleTreeBaseTile m_SingleTreeBaseTile;
    public SingleTreeTopTile  m_SingleTreeTopTile;

    //-----------------------------------------------------------------------------------
    public override void Paint( GridLayout grid_layout, GameObject brush_target, Vector3Int position )
    {
      Tilemap tilemap = brush_target.GetComponent<Tilemap>();
      Vector3Int up_pos   = position + new Vector3Int( 0,  1, 0 );
      Vector3Int down_pos = position + new Vector3Int( 0, -1, 0 );

      // analyze surrounding tiles to determine subtype
      CustomTile up_tile   = tilemap.GetTile<CustomTile>( up_pos );
      CustomTile down_tile = tilemap.GetTile<CustomTile>( down_pos );

      // if no multi-trees below we know it's a stump-type
      if ( down_tile == null || ( down_tile.m_Type & CustomTile.Type.MultiTree ) == 0 )
      {
        PaintStump( tilemap, position );
      }
      // else, if multi-tree below and no multi-tree above, it's a top type
      else if ( up_tile == null || ( up_tile.m_Type & CustomTile.Type.MultiTree ) == 0 )
      {
        PaintTop( tilemap, position );
      }
      // else, it's a mid-type
      else
      {
        PaintMid( tilemap, position );
      }
    }

    //-----------------------------------------------------------------------------------
    private void PaintStump( Tilemap tilemap, Vector3Int position )
    {
      // first get state of surrounding tiles.
      Vector3Int up_pos  = position + new Vector3Int( 0,  1, 0 );
      CustomTile up_tile = tilemap.GetTile<CustomTile>( up_pos );

      // Are we extending a stump downward?
      CustomTile.Type multi_base_type = CustomTile.Type.MultiTree | CustomTile.Type.MultiTreeBase;
      if ( up_tile != null && ( up_tile.m_Type & multi_base_type ) == multi_base_type )
      {
        Vector3Int two_up      = up_pos + new Vector3Int( 0, 1, 0 );
        CustomTile two_up_tile = tilemap.GetTile<CustomTile>( two_up );

        // reassign previous stump
        if ( two_up_tile == null || !two_up_tile.IsType( CustomTile.Type.MultiTree ) )
        {
          PaintTop( tilemap, up_pos );
        }
        else
        {
          PaintMid( tilemap, up_pos );
        }
      }

      // Paint this tile
      tilemap.SetTile( position, m_BaseTile );

      // now set periphery
      Vector3Int left_pos  = position + new Vector3Int( -1, 0, 0 );
      Vector3Int right_pos = position + new Vector3Int(  1, 0, 0 );

      CustomTile left_tile  = tilemap.GetTile<CustomTile>( left_pos  );
      CustomTile right_tile = tilemap.GetTile<CustomTile>( right_pos );

      if ( left_tile == null )
      {
        tilemap.SetTile( left_pos, m_BaseLeftPeripheryTile );
      }
      else if ( left_tile.IsType( CustomTile.Type.SingleTree ) )
      {
        CustomTile ct = left_tile.IsType( CustomTile.Type.MultiTreeBaseRightPeriphery )
                          ? m_SingleComboBaseComboPeripheryTile
                          : m_SingleComboBaseLeftPeripheryTile;

        if ( left_tile.IsType( CustomTile.Type.SingleTreeBase ) )
        {
          ct = left_tile.IsType(CustomTile.Type.MultiTreeBaseRightPeriphery)
                          ? m_SingleBaseComboBaseComboPeripheryTile
                          : m_SingleBaseComboBaseLeftPeripheryTile;
        }
        else if ( left_tile.IsType( CustomTile.Type.SingleTreeTop ) )
        {
          ct = left_tile.IsType(CustomTile.Type.MultiTreeBaseRightPeriphery)
                          ? m_SingleTopComboBaseComboPeripheryTile
                          : m_SingleTopComboBaseLeftPeripheryTile;
        }

        tilemap.SetTile( left_pos, ct );
      }
      else if ( left_tile.IsType( CustomTile.Type.MultiTreeBaseRightPeriphery ) )
      {
        tilemap.SetTile( left_pos, m_BaseComboPeripheryTile );
      }


      if ( right_tile == null )
      {
        tilemap.SetTile( right_pos, m_BaseRightPeripheryTile );
      }
      else if ( right_tile.IsType( CustomTile.Type.SingleTree ) ) 
      {
        CustomTile ct = right_tile.IsType(CustomTile.Type.MultiTreeBaseLeftPeriphery)
                          ? m_SingleComboBaseComboPeripheryTile
                          : m_SingleComboBaseRightPeripheryTile;

        if ( right_tile.IsType( CustomTile.Type.SingleTreeBase ) )
        {
          ct = right_tile.IsType(CustomTile.Type.MultiTreeBaseLeftPeriphery)
                          ? m_SingleBaseComboBaseComboPeripheryTile
                          : m_SingleBaseComboBaseRightPeripheryTile;
        }
        else if ( right_tile.IsType( CustomTile.Type.SingleTreeTop ) )
        {
          ct = right_tile.IsType(CustomTile.Type.MultiTreeBaseLeftPeriphery)
                          ? m_SingleTopComboBaseComboPeripheryTile
                          : m_SingleTopComboBaseRightPeripheryTile;
        }

        tilemap.SetTile( right_pos, ct );
      }
      else if ( right_tile.IsType( CustomTile.Type.MultiTreeBaseLeftPeriphery ) )
      {
        tilemap.SetTile( right_pos, m_BaseComboPeripheryTile );
      }
    }

    //-----------------------------------------------------------------------------------
    private void PaintTop( Tilemap tilemap, Vector3Int position )
    {
      Vector3Int down_pos  = position + new Vector3Int( 0, -1, 0 );
      CustomTile down_tile = tilemap.GetTile<CustomTile>( down_pos );

      // Are we extending a top upward?
      CustomTile.Type multi_top_type = CustomTile.Type.MultiTree | CustomTile.Type.MultiTreeTop;
      if ( down_tile != null && down_tile.IsType( multi_top_type ) )
      {
        PaintMid( tilemap, down_pos );
      }

      tilemap.SetTile( position, m_TopTile );

      // now set periphery
      Vector3Int left_pos  = position + new Vector3Int( -1, 0, 0 );
      Vector3Int right_pos = position + new Vector3Int(  1, 0, 0 );

      CustomTile left_tile  = tilemap.GetTile<CustomTile>( left_pos );
      CustomTile right_tile = tilemap.GetTile<CustomTile>( right_pos );

      if ( left_tile == null )
      {
        tilemap.SetTile( left_pos, m_TopLeftPeripheryTile );
      }
      else if ( left_tile.IsType( CustomTile.Type.SingleTree ) )
      {
        CustomTile ct = left_tile.IsType( CustomTile.Type.MultiTreeTopRightPeriphery )
                          ? m_SingleComboTopComboPeripheryTile
                          : m_SingleComboTopLeftPeripheryTile;

        if ( left_tile.IsType( CustomTile.Type.SingleTreeBase ) )
        {
          ct = left_tile.IsType( CustomTile.Type.MultiTreeTopRightPeriphery )
              ? m_SingleBaseComboTopComboPeripheryTile
              : m_SingleBaseComboTopLeftPeripheryTile;
        }
        else if ( left_tile.IsType( CustomTile.Type.SingleTreeTop ) )
        {
          ct = left_tile.IsType( CustomTile.Type.MultiTreeTopRightPeriphery )
              ? m_SingleTopComboTopComboPeripheryTile
              : m_SingleTopComboTopLeftPeripheryTile;
        }

        tilemap.SetTile( left_pos, ct );
      }
      else if ( left_tile.IsType(CustomTile.Type.MultiTreeBaseLeftPeriphery ) )
      {
        tilemap.SetTile( left_pos, m_TopLeftPeripheryTile );
      }
      else if ( left_tile.IsType( CustomTile.Type.MultiTreeTopRightPeriphery ) )
      {
        tilemap.SetTile( left_pos, m_TopComboPeripheryTile );
      }

      if ( right_tile == null )
      {
        tilemap.SetTile( right_pos, m_TopRightPerirpheryTile );
      }
      else if ( right_tile.IsType( CustomTile.Type.SingleTree ) )
      {
        CustomTile ct = right_tile.IsType( CustomTile.Type.MultiTreeTopLeftPeriphery )
                          ? m_SingleComboTopComboPeripheryTile
                          : m_SingleComboTopRightPeripheryTile;
        if ( right_tile.IsType( CustomTile.Type.SingleTreeBase ) )
        {
          ct = right_tile.IsType( CustomTile.Type.MultiTreeTopLeftPeriphery )
              ? m_SingleBaseComboTopComboPeripheryTile
              : m_SingleBaseComboTopRightPeripheryTile;
        }
        else if ( right_tile.IsType( CustomTile.Type.SingleTreeTop ) )
        {
          ct = right_tile.IsType( CustomTile.Type.MultiTreeTopLeftPeriphery )
              ? m_SingleTopComboTopComboPeripheryTile
              : m_SingleTopComboTopRightPeripheryTile;
        }

        tilemap.SetTile( right_pos, ct );
      }
      else if ( right_tile.IsType( CustomTile.Type.MultiTreeBaseRightPeriphery ) )
      {
        tilemap.SetTile( right_pos, m_TopRightPerirpheryTile );
      }
      else if ( right_tile.IsType( CustomTile.Type.MultiTreeTopLeftPeriphery ) )
      {
        tilemap.SetTile( right_pos, m_TopComboPeripheryTile );
      }
    }

    //-----------------------------------------------------------------------------------
    private void PaintMid( Tilemap tilemap, Vector3Int position )
    {
      tilemap.SetTile( position, m_MidTile );
      
      // now unset periphery
      Vector3Int left_pos = position + new Vector3Int(-1, 0, 0);
      Vector3Int right_pos = position + new Vector3Int(1, 0, 0);

      CustomTile left_tile = tilemap.GetTile<CustomTile>(left_pos);
      CustomTile right_tile = tilemap.GetTile<CustomTile>(right_pos);

      if ( left_tile != null )
      {
        CustomTile.Type left_periphery_type = CustomTile.Type.MultiTreeBaseLeftPeriphery | CustomTile.Type.MultiTreeTopLeftPeriphery;
        if ( left_tile.IsAnyType( left_periphery_type ) )
        {
          tilemap.SetTile( left_pos, TileForRemoveLeftPeriphery( left_tile ) );
        }
      }

      if ( right_tile != null )
      {
        CustomTile.Type right_periphery_type = CustomTile.Type.MultiTreeBaseRightPeriphery | CustomTile.Type.MultiTreeTopRightPeriphery;
        if ( right_tile.IsAnyType( right_periphery_type ) )
        {
          tilemap.SetTile( right_pos, TileForRemoveRightPeriphery( right_tile ) );
        }
      }
    }

    //-----------------------------------------------------------------------------------
    private CustomTile TileForRemoveLeftPeriphery( CustomTile left_tile )
    {
      bool is_right_base_periphery = left_tile.IsType( CustomTile.Type.MultiTreeBaseRightPeriphery );
      bool is_right_top_periphery  = left_tile.IsType( CustomTile.Type.MultiTreeTopRightPeriphery  );

      if ( left_tile.IsType( CustomTile.Type.SingleTree ) )
      {
        if ( left_tile.IsType( CustomTile.Type.SingleTreeBase ) )
        {
          if ( is_right_base_periphery )
          {
            return m_SingleBaseComboBaseRightPeripheryTile;
          }
          else if ( is_right_top_periphery )
          {
            return m_SingleBaseComboTopRightPeripheryTile;
          }

          return m_SingleTreeBaseTile;
        }
        else if ( left_tile.IsType( CustomTile.Type.SingleTreeTop ) )
        {
          if ( is_right_base_periphery )
          {
            return m_SingleTopComboBaseRightPeripheryTile;
          }
          else if ( is_right_top_periphery )
          {
            return m_SingleTopComboTopRightPeripheryTile;
          }

          return m_SingleTreeTopTile;
        }

        if ( is_right_base_periphery )
        {
          return m_SingleComboBaseRightPeripheryTile;
        }
        else if ( is_right_top_periphery )
        {
          return m_SingleComboTopRightPeripheryTile;
        }

        return m_SingleTreeTile;
      }
      else if ( is_right_base_periphery )
      {
        return m_BaseRightPeripheryTile;
      }
      else if ( is_right_top_periphery )
      {
        return m_TopRightPerirpheryTile;
      }

      return null;
    }

    //-----------------------------------------------------------------------------------
    private CustomTile TileForRemoveRightPeriphery( CustomTile right_tile )
    {
      bool is_left_base_periphery = right_tile.IsType( CustomTile.Type.MultiTreeBaseLeftPeriphery );
      bool is_left_top_periphery  = right_tile.IsType( CustomTile.Type.MultiTreeTopLeftPeriphery  );

      if ( right_tile.IsType( CustomTile.Type.SingleTree ) )
      {
        if ( right_tile.IsType( CustomTile.Type.SingleTreeBase ) )
        {
          if ( is_left_base_periphery )
          {
            return m_SingleBaseComboBaseLeftPeripheryTile;
          }
          else if ( is_left_top_periphery )
          {
            return m_SingleBaseComboTopLeftPeripheryTile;
          }

          return m_SingleTreeBaseTile;
        }
        else if ( right_tile.IsType( CustomTile.Type.SingleTreeTop ) )
        {
          if ( is_left_base_periphery )
          {
            return m_SingleTopComboBaseLeftPeripheryTile;
          }
          else if ( is_left_top_periphery )
          {
            return m_SingleTopComboTopLeftPeripheryTile;
          }

          return m_SingleTreeTopTile;
        }

        if ( is_left_base_periphery )
        {
          return m_SingleComboBaseLeftPeripheryTile;
        }
        else if ( is_left_top_periphery )
        {
          return m_SingleComboTopLeftPeripheryTile;
        }

        return m_SingleTreeTile;
      }
      else if ( is_left_base_periphery )
      {
        return m_BaseLeftPeripheryTile;
      }
      else if ( is_left_top_periphery )
      {
        return m_TopLeftPeripheryTile;
      }

      return null;
    }

    //-----------------------------------------------------------------------------------
    [MenuItem("Assets/Create/CustomTilemapEditor/MultiTree Brush")]
    public static void CreateBrush()
    {
      string path = EditorUtility.SaveFilePanelInProject( "Save MultiTree Brush", "New MultiTree Brush", "Asset", "Save MultiTree Brush", "Assets/CustomTilemapEditor");
      if (path == "")
        return;
      AssetDatabase.CreateAsset( ScriptableObject.CreateInstance<MultiTreeBrush>(), path );
    }
  }

  //-----------------------------------------------------------------------------------
  [CustomEditor(typeof(MultiTreeBrush))]
  public class  MultiTreeBrushEditor : GridBrushEditor
    {
    public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
    {
      base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
    }
  }
}