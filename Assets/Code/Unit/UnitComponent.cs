using UnityEngine;

public class UnitComponent : MonoBehaviour
{
  public GameObject m_SpawnOnDeathPrefab;

  void Start()
  {
    HealthChannel health_channel = GetComponent<HealthChannel>();
    health_channel.OnHealthEvent += ( HealthEvent evt ) =>
    {
      if ( evt.m_Type == HealthEvent.Type.Dead )
      {
        Instantiate( m_SpawnOnDeathPrefab, transform.position, Quaternion.identity );
        Destroy( gameObject );
      }
    };
  }
};