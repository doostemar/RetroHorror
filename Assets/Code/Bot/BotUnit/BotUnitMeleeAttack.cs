using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitMeleeAttack : MonoBehaviour
{
  public Collider2D m_Collider;
  public float      m_AttackValue;

  BotUnitChannel      m_UnitChannel;
  LayerMask           m_EnemyUnitMask;
  HashSet<Collider2D> m_AlreadyHitColliders;

  void Start()
  {
    m_EnemyUnitMask = LayerMask.GetMask( new string[] { "EnemyUnit" } );
    m_UnitChannel   = GetComponent<BotUnitChannel>();
    m_UnitChannel.OnUnitEvent += OnUnitEvent;
    enabled = false;
  }

  void OnUnitEvent( BotUnitEvent evt )
  {
    if ( evt.m_Type == BotUnitEvent.Type.Attack )
    {
      m_AlreadyHitColliders = new HashSet<Collider2D>();
      enabled = true;
    }
    else if ( evt.m_Type == BotUnitEvent.Type.AttackFinished )
    {
      enabled = false;
    }
  }

  private void FixedUpdate()
  {
    ContactFilter2D filter2D = new ContactFilter2D();
    filter2D.useLayerMask = true;
    filter2D.layerMask    = m_EnemyUnitMask;
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
