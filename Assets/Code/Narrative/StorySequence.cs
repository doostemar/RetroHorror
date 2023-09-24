using UnityEngine;
using UnityEngine.SceneManagement;

public class StorySequence : MonoBehaviour
{
  public string m_NextScene;

  void Update()
  {
    if ( Input.GetButtonUp( "Cancel" ) || Input.GetButtonUp( "Submit" ) || Input.GetButtonUp( "Back" ) )
    {
      NextScene();
    }
  }

  public void NextScene()
  {
    SceneManager.LoadScene(m_NextScene);
  }
}
