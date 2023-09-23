using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
  private void Awake()
  {
    Game.Init();
  }

  private void OnDestroy()
  {
    Game.Shutdown();
  }
}
