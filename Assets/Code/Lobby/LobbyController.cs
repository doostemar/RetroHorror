using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
  public void StartGame()
  {
    SceneManager.LoadScene( "StoryIntro" );
  }

  public void ShowHowToPlay()
  {
    SceneManager.LoadScene( "HowToPlayScene" );
  }
}
