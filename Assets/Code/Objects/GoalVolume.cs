using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalVolume : MonoBehaviour
{
  public string m_NextScene;

  Collider2D m_Collider;
  int        m_HeroMask;
  bool       m_Triggered;

  private void Start()
  {
    m_Collider = GetComponent<Collider2D>();
    m_HeroMask = LayerMask.NameToLayer( "Hero" );
    m_Triggered = false;
  }

  private void FixedUpdate()
  {
    if (!m_Triggered)
    {
      ContactFilter2D contact_filter = new ContactFilter2D();
      contact_filter.layerMask = m_HeroMask;
      List<Collider2D> results = new List<Collider2D>();
      int num_overlaps = m_Collider.OverlapCollider(contact_filter, results);

      if (num_overlaps > 0)
      {
        m_Triggered = true;
        LoadLevelSystem lls = Game.GetGameController().GetComponent<LoadLevelSystem>();
        lls.LoadLevel(m_NextScene);
      }
    }
  }
}
