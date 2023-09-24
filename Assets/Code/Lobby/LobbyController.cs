using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
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
