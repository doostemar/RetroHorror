using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
  public string m_FirstSceneName;

  void Start()
  {
    DontDestroyOnLoad( gameObject );
    SceneManager.LoadScene( m_FirstSceneName );
  }
}
