using UnityEngine;
using UnityEngine.SceneManagement;

public class StorySequence : MonoBehaviour
{
  public string m_NextScene;
  public bool   m_AutoTransition;

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

  public void AnimationFinished()
  {
    if ( m_AutoTransition )
    {
      NextScene();
    }
  }
}
