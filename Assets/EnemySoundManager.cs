using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
  public AudioSource m_AudioSource;
  public AudioClip m_Attack;
  public AudioClip m_Death;


  public void PlayAttack()
  {
    m_AudioSource.clip = m_Attack;
    m_AudioSource.Play();
  }

  public void PlayDeath()
  {
    m_AudioSource.clip = m_Death;
    m_AudioSource.Play();
  }
}
