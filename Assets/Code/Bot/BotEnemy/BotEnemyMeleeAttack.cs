using System.Collections.Generic;
using UnityEngine;

public class BotEnemyMeleeAttack : MonoBehaviour
{
  public Collider2D   m_Collider;
  public float        m_AttackValue;

  BotEnemyChannel     m_Channel;
  LayerMask           m_HeroUnitMask;
  Collider2D          m_AlreadyHitCollider;

  void Start()
  {
    m_HeroUnitMask = LayerMask.GetMask(new string[] { "HeroUnit" });

    m_Channel = GetComponent<BotEnemyChannel>();

    m_Channel.OnEnemyEvent += OnEnemyEvent;
    enabled = false;
  }

  void OnEnemyEvent( BotEnemyEvent evt )
  {
    if ( evt.m_Type == BotEnemyEvent.Type.Attack )
    {
      m_AlreadyHitCollider = null;
      enabled = true;
    }
    else if ( evt.m_Type == BotEnemyEvent.Type.AttackFinished )
    {
      enabled = false;
    }
  }

  void FixedUpdate()
  {
    if ( m_Collider.enabled == false || m_AlreadyHitCollider != null )
    {
      return;
    }

    ContactFilter2D filter2D = new ContactFilter2D();
    filter2D.useLayerMask = true;
    filter2D.layerMask    = m_HeroUnitMask;
    List<Collider2D> overlaps = new List<Collider2D>();
    m_Collider.OverlapCollider( filter2D, overlaps );

    if ( overlaps.Count > 0 )
    {
      Collider2D overlap = overlaps[0];
      HealthChannel health = overlap.gameObject.GetComponent<HealthChannel>();
      if ( health != null )
      {
        HealthEvent evt = ScriptableObject.CreateInstance<HealthEvent>();
        evt.m_Direction = overlap.transform.position - m_Collider.transform.position;
        evt.m_Type      = HealthEvent.Type.Damage;
        evt.m_Value     = m_AttackValue;

        health.RaiseHealthEvent( evt );
      }
      m_AlreadyHitCollider = overlap;
    }
  }
}
