using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
  [Flags]
  public enum Type
  {
    Unset          = 0,
    SingleTree     = 1,
    SingleTreeBase = 2,
    SingleTreeTop  = 4
  }

  public Type m_Type = Type.Unset;

  public Type GetTileType( ITilemap tilemap, Vector3Int position )
  {
    TileBase tb = (TileBase)tilemap.GetTile( position );
    if ( tb is CustomTile )
    {
      CustomTile ct = (CustomTile)tb;
      return ct.m_Type;
    }
    return Type.Unset;
  }
}
