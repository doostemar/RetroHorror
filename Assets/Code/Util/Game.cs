using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
  static GameObject    s_GameController;
  static bool          s_Destroyed = false;
  static Camera        s_RenderCamera = null;
  static GlobalFadeout s_GlobalFadeout = null;

  public static Camera GetRenderCamera()
  {
    if ( s_RenderCamera == null )
    { 
      s_RenderCamera = GetGameController().GetComponent<RenderCameraRef>().m_RenderCamera;
    }
    return s_RenderCamera;
  }

  public static GameObject GetGameController()
  {
    if ( s_GameController == null && s_Destroyed == false )
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

  static public void Init()
  {
    s_Destroyed = false;
  }

  static public void Shutdown()
  {
    s_Destroyed = true;
    s_GlobalFadeout = null;
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

  public static void RegisterGlobalFadeout( GlobalFadeout gf )
  {
    s_GlobalFadeout = gf;
  }

  public static GlobalFadeout GetGlobalFadeout()
  {
    return s_GlobalFadeout;
  }
}
