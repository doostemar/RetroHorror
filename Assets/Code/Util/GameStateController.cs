using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
  private void OnDestroy()
  {
    Game.Shutdown();
  }
}
