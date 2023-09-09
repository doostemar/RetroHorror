using UnityEngine;

public class MathHelper
{
  public static Vector2 Rotate2D( Vector2 dir, float angle )
  {
    return new Vector2(
      dir.x * Mathf.Cos( angle ) - dir.y * Mathf.Sin( angle ),
      dir.x * Mathf.Sin( angle ) + dir.y * Mathf.Cos( angle )
    );
  }

  public static Bounds GrowBounds( Bounds a, Bounds b )
  {
    Bounds new_bounds = new Bounds();
    new_bounds.min = Vector3.Min( a.min, b.min );
    new_bounds.max = Vector3.Max( a.max, b.max );
    return new_bounds;
  }

  public static BoundsInt GrowBounds( BoundsInt a, BoundsInt b )
  {
    BoundsInt new_bounds = new BoundsInt();
    new_bounds.min = Vector3Int.Min( a.min, b.min );
    new_bounds.max = Vector3Int.Max( a.max, b.max );
    return new_bounds;
  }
}
