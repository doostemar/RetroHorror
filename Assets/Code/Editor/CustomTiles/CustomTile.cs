using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
  [Flags]
  public enum Type
  {
    Unset                       = 0x00000000,
    SingleTree                  = 0x00000001,
    SingleTreeBase              = 0x00000002,
    SingleTreeTop               = 0x00000004,
    MultiTree                   = 0x00000008,
    MultiTreeBase               = 0x00000010,
    MultiTreeTop                = 0x00000020,
    MultiTreeTopLeftPeriphery   = 0x00000040,
    MultiTreeTopRightPeriphery  = 0x00000080,
    MultiTreeBaseLeftPeriphery  = 0x00000100,
    MultiTreeBaseRightPeriphery = 0x00000200,
  }

  [SerializeField]
  public Type m_Type = Type.Unset;

  public Type GetTileType( ITilemap tilemap, Vector3Int position )
  {
    CustomTile ct = tilemap.GetTile<CustomTile>( position );
    if ( ct != null )
    {
      return ct.m_Type;
    }
    return Type.Unset;
  }

  public bool IsType( Type type )
  {
    return ( m_Type & type ) == type;
  }

  public bool IsAnyType( Type type )
  {
    return ( m_Type & type ) != 0;
  }
}
