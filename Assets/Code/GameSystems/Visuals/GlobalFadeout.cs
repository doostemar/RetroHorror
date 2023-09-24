using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFadeout : MonoBehaviour
{
  Animator m_Animator;

  bool     m_FadedOut;
  int      m_FadeOutStateId;
  int      m_FadeInStateId;

  Action   m_FinishedCallback;

  void Start()
  {
    m_Animator = GetComponent<Animator>();
    m_FadeOutStateId = Animator.StringToHash( "FadeOut" );
    m_FadeInStateId  = Animator.StringToHash( "FadeIn"  );
    m_FadedOut       = false;
    Game.RegisterGlobalFadeout( this );
  }

  public void NotifyFadeFinished()
  {
    if ( m_FinishedCallback != null )
    {
      m_FinishedCallback();
      m_FinishedCallback = null;
    }
  }

  public void FadeOut( float time_s, Action finished_callback = null )
  {
    if ( m_FinishedCallback != null )
    {
      m_FinishedCallback();
    }

    if ( !m_FadedOut )
    {
      m_FinishedCallback = finished_callback;
      m_FadedOut = true;
      m_Animator.speed = 1f / time_s;
      m_Animator.Play( m_FadeOutStateId );
    }
  }

  public void FadeIn( float time_s, Action finished_callback = null )
  {
    if ( m_FinishedCallback != null )
    {
      m_FinishedCallback();
    }

    if ( m_FadedOut )
    {
      m_FinishedCallback = finished_callback;
      m_FadedOut = false;
      m_Animator.speed = 1f / time_s;
      m_Animator.Play( m_FadeInStateId );
    }
  }
}
