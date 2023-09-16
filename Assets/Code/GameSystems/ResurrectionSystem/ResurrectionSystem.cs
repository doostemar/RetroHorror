using UnityEngine;

public class ResurrectionSystem : MonoBehaviour
{
  [HideInInspector]
  public ResurrectionEventChannel m_ResChannel;
  public GameObject               m_SealSpritePrefab;

  private GameObject m_SealGameObj;
  private float      m_FadeTimeS;
  private float      m_TimeSinceCastS;
  private bool       m_Fading;

  private void Awake()
  {
    m_ResChannel = gameObject.AddComponent<ResurrectionEventChannel>();
  }

  void Start()
  {
    m_ResChannel.OnResurrectionEvent += OnResEvent;

    m_Fading = false;
  }

  private void OnDisable()
  {
    if ( m_SealGameObj != null )
    {
      Destroy( m_SealGameObj );
    }
  }

  void Update()
  {
    if ( m_Fading )
    {
      m_TimeSinceCastS += Time.deltaTime;

      float fade_amt = 1f - ( m_TimeSinceCastS / m_FadeTimeS );
      {
        Color fade_cast = Color.white;
        fade_cast.a = fade_amt;
      }

      if ( m_TimeSinceCastS > m_FadeTimeS )
      {
        m_Fading = false;
        Destroy( m_SealGameObj );
      }
    }
  }

  void OnResEvent( ResurrectionEvent res_event )
  {
    if ( m_SealGameObj != null )
    {
      m_SealGameObj.transform.position = res_event.m_Position;
    }

    if ( res_event.m_Type == ResurrectionEvent.Type.Display )
    {
      if (m_SealGameObj == null)
      {
        m_SealGameObj = Instantiate(m_SealSpritePrefab, transform);
      }
      m_SealGameObj.transform.position = res_event.m_Position;
    }
    else if ( res_event.m_Type == ResurrectionEvent.Type.Cast )
    {
      Animator anim = m_SealGameObj.GetComponent<Animator>();
      anim.Play( "ResSealAnimCast" );
      m_FadeTimeS          = res_event.m_TimeToDisplay;
      m_TimeSinceCastS     = 0f;
      m_Fading             = true;
    }
  }
}
