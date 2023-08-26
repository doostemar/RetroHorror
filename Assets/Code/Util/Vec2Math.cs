using UnityEngine;

public class Vec2Math
{
  public static Vector2 Rotate2D( Vector2 dir, float angle )
  {
    return new Vector2(
      dir.x * Mathf.Cos( angle ) - dir.y * Mathf.Sin( angle ),
      dir.x * Mathf.Sin( angle ) + dir.y * Mathf.Cos( angle )
    );
  }
}
