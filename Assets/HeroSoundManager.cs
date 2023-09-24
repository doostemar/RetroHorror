using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroSoundManager : MonoBehaviour
{
  public AudioSource m_AudioSource;
  public AudioClip m_leftStep;
  public AudioClip m_rightStep;
  public AudioClip m_SummonMagic;
  public AudioClip m_SummonVoice;
  public AudioClip m_Death;


  public void PlayLeftFoot()
  {
    m_AudioSource.clip = m_leftStep;
    m_AudioSource.Play();
  }

  public void PlayRightFoot()
  {
    m_AudioSource.clip = m_rightStep;
    m_AudioSource.Play();
  }

  public void PlaySummon()
  {
    m_AudioSource.clip = m_SummonMagic;
    m_AudioSource.Play();
  }

  public void PlaySummonVoice()
  {
    m_AudioSource.clip = m_SummonVoice;
    m_AudioSource.Play();
  }

  public void PlayDeath()
  {
    m_AudioSource.clip = m_Death;
    m_AudioSource.Play();
  }

}
