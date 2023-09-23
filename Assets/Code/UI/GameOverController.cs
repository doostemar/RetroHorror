using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
  public void Continue()
  {
    SceneManager.LoadScene( SceneManager.GetActiveScene().name );
  }

  public void Quit()
  {
    SceneManager.LoadScene( "Lobby" );
  }
}
