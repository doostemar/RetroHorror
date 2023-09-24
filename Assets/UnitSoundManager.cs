using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSoundManager : MonoBehaviour
{
  public AudioSource m_AudioSource;
  public AudioClip m_Attack1;
  public AudioClip m_Attack2;
  public AudioClip m_Res;
  public AudioClip m_Death;


  public void PlayAttack1()
  {
    m_AudioSource.clip = m_Attack1;
    m_AudioSource.volume = .75f;
    m_AudioSource.Play();
    m_AudioSource.volume = 1f;
  }

  public void PlayAttack2()
  {
    m_AudioSource.clip = m_Attack2;
    m_AudioSource.Play();
  }

  public void PlayRes()
  {
    m_AudioSource.clip = m_Res;
    m_AudioSource.Play();
  }

  public void PlayDeath()
  {
    m_AudioSource.clip = m_Death;
    m_AudioSource.Play();
  }
}
