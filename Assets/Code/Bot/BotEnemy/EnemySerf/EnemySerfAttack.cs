using System.Collections.Generic;
using UnityEngine;

public class EnemySerfAttack : MonoBehaviour
{
  public Collider2D   m_Collider;
  public float        m_AttackValue;

  BotEnemyChannel     m_Channel;
  LayerMask           m_HeroUnitMask;
  HashSet<Collider2D> m_AlreadyHitColliders;

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
      m_AlreadyHitColliders = new HashSet<Collider2D>();
      enabled = true;
    }
    else if ( evt.m_Type == BotEnemyEvent.Type.AttackFinished )
    {
      enabled = false;
    }
  }

  void FixedUpdate()
  {
    ContactFilter2D filter2D = new ContactFilter2D();
    filter2D.useLayerMask = true;
    filter2D.layerMask    = m_HeroUnitMask;
    List<Collider2D> overlaps = new List<Collider2D>();
    m_Collider.OverlapCollider( filter2D, overlaps );

    foreach ( Collider2D overlap in overlaps )
    {
      if ( m_AlreadyHitColliders.Contains( overlap ) == false )
      {
        HealthChannel health = overlap.gameObject.GetComponent<HealthChannel>();
        if ( health != null )
        {
          HealthEvent evt = ScriptableObject.CreateInstance<HealthEvent>();
          evt.m_Direction = overlap.transform.position - m_Collider.transform.position;
          evt.m_Type      = HealthEvent.Type.Damage;
          evt.m_Value     = m_AttackValue;

          health.RaiseHealthEvent( evt );
        }
        m_AlreadyHitColliders.Add( overlap );
      }
    }
  }
}
