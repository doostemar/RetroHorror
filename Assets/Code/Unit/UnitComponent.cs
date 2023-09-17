using UnityEngine;

public class UnitComponent : MonoBehaviour
{
  public GameObject m_SpawnOnDeathPrefab;

  private GameObject m_BloodParticleAsset;

  void Start()
  {
    m_BloodParticleAsset = Resources.Load<GameObject>( "Prefabs/Blood Particles" );
    HealthChannel health_channel = GetComponent<HealthChannel>();
    health_channel.OnHealthEvent += ( HealthEvent evt ) =>
    {
      if ( evt.m_Type == HealthEvent.Type.Dead )
      {
        Quaternion direction = Quaternion.LookRotation( evt.m_Direction );
        Instantiate( m_BloodParticleAsset, transform.position, direction );
        Instantiate( m_SpawnOnDeathPrefab, transform.position, Quaternion.identity );
        Destroy( gameObject );
      }
    };
  }
};