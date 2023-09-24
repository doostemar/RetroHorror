using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroSoundManager : MonoBehaviour
{
  public AudioSource m_AudioSource;
  public AudioClip   m_leftStep;
  public AudioClip   m_rightStep;
  public AudioClip   m_SummonMagic;
  public AudioClip   m_SummonVoice;
  public AudioClip   m_Damage;
  public AudioClip   m_Death;

  private bool m_IsDead = false;

  private void Start()
  {
    HealthChannel health_channel = GetComponent<HealthChannel>();
    health_channel.OnHealthEvent += OnHealthEvent;
  }

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

  private void OnHealthEvent(HealthEvent health_evt)
  {
    if (health_evt.m_Type == HealthEvent.Type.Dead && m_IsDead == false)
    {
      m_IsDead = true;
      m_AudioSource.clip = m_Death;
      m_AudioSource.Play();
    }
    else if ( health_evt.m_Type == HealthEvent.Type.Damage && m_IsDead == false )
    {
      m_AudioSource.clip = m_Damage;
      m_AudioSource.Play();
    }
  }
}
