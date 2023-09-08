using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
  static GameObject s_GameController;

  public static GameObject GetGameController()
  {
    if ( s_GameController == null )
    { 
      GameObject[] game_controllers = GameObject.FindGameObjectsWithTag("GameController");

      if ( game_controllers.Length != 1 )
      { 
        Debug.LogError( "Require exactly one GameController in the scene!" );
        return null;
      }

      s_GameController = game_controllers[0];
    }

    return s_GameController;
  }

  public static bool IsRunning()
  {
    if ( s_GameController == null )
    {
      GameObject[] game_controllers = GameObject.FindGameObjectsWithTag("GameController");
      return game_controllers.Length > 0;
    }
    return true;
  }
}
