using UnityEngine;

public class Health : MonoBehaviour
{
  public float          m_MaxHealth;
  public HealthBar      m_HealthBar;
  public bool           m_IsInvincible;

  private float         m_CurrentHealth;
  private HealthChannel m_HealthChannel;

  void Start()
  {
    m_CurrentHealth = m_MaxHealth;
    m_HealthBar.SetMaxHealth(m_CurrentHealth);

    m_HealthChannel = GetComponent<HealthChannel>();
    m_HealthChannel.OnHealthEvent += ( HealthEvent evt ) =>
    {
      switch ( evt.m_Type )
      {
        case HealthEvent.Type.Damage:
          TakeDamage( evt.m_Value, evt.m_Direction );
          break;
        case HealthEvent.Type.Heal:
          TakeDamage( -evt.m_Value, evt.m_Direction );
          break;
        case HealthEvent.Type.InvincibleStart:
          m_IsInvincible = true;
          break;
        case HealthEvent.Type.InvincibleEnd:
          m_IsInvincible = false;
          break;
      }
    };
  }

  void TakeDamage( float damage, Vector3 direction )
  {
    m_CurrentHealth = Mathf.Clamp( m_CurrentHealth - damage, 0, m_MaxHealth );
    m_HealthBar.SetHealth( m_CurrentHealth );

    if ( m_CurrentHealth == 0 )
    {
      HealthEvent evt = ScriptableObject.CreateInstance<HealthEvent>();
      evt.m_Type      = HealthEvent.Type.Dead;
      evt.m_Direction = direction;
      m_HealthChannel.RaiseHealthEvent( evt );
    }
  }
}
