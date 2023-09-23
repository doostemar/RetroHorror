using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayController : MonoBehaviour
{
  public GameObject[] m_Pages;

  private int m_ActivePage;

  void Start()
  {
    foreach ( GameObject page in m_Pages )
    {
      page.SetActive( false );
    }

    m_ActivePage = 0;
    if (m_Pages.Length > 0)
    {
      m_Pages[ 0 ].SetActive( true );
    }
  }

  void Update()
  {
    if ( Input.GetButtonUp( "Submit" ) )
    {
      if ( m_ActivePage == m_Pages.Length - 1 )
      {
        ReturnToLobby();
      }
      else
      {
        SetActivePage( m_ActivePage + 1 );
      }
    }

    if ( Input.GetButtonUp( "Back" ) )
    {
      if ( m_ActivePage == 0 )
      {
        ReturnToLobby();
      }
      else
      {
        SetActivePage( m_ActivePage - 1 );
      }
    }

    if ( Input.GetButtonUp( "Cancel" ) )
    {
      ReturnToLobby();
    }
  }

  void SetActivePage( int idx )
  {
    m_Pages[ m_ActivePage ].SetActive( false );
    m_ActivePage = idx;
    m_Pages[ m_ActivePage ].SetActive( true  );
  }

  void ReturnToLobby()
  {
    SceneManager.LoadScene( "Lobby" );
  }
}
