using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSystem : MonoBehaviour
{
  string m_RequestedName;

  public void LoadLevel( string name )
  {
    m_RequestedName = name;
    Game.GetGlobalFadeout().FadeOut( 1, () =>
    {
      SceneManager.LoadScene( m_RequestedName );
    });
  }
}
