using UnityEngine;

public class ResurrectionSystem : MonoBehaviour
{
  public ResurrectionEventChannel m_ResChannel;

  private DebugCircleRenderer m_AoeCircle;
  private float               m_FadeTimeS;
  private float               m_TimeSinceCastS;
  private bool                m_Fading;

  // red
  private Color m_AoeColor = new Color( 177f / 255f, 52f / 255f, 63f / 255f );

  private void Awake()
  {
    m_ResChannel = gameObject.AddComponent<ResurrectionEventChannel>();
  }

  void Start()
  {
    if ( m_AoeCircle == null )
    { 
      m_AoeCircle              = gameObject.AddComponent<DebugCircleRenderer>();
      m_AoeCircle.m_Color      = m_AoeColor;
      m_AoeCircle.m_PointCount = 35;
    }
    m_AoeCircle.enabled = false;
    m_ResChannel.OnResurrectionEvent += OnResEvent;

    m_Fading = false;
  }

  private void OnDisable()
  {
    m_AoeCircle.enabled = false;
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
        m_AoeCircle.m_Color = fade_cast;
      }

      if ( m_TimeSinceCastS > m_FadeTimeS )
      {
        m_Fading = false;
        m_AoeCircle.enabled = false;
      }
    }
  }

  void OnResEvent( ResurrectionEvent res_event )
  {
    m_AoeCircle.enabled  = true;
    m_AoeCircle.m_Center = res_event.m_Position;
    m_AoeCircle.m_Radius = res_event.m_Radius;

    if ( res_event.m_Type == ResurrectionEvent.Type.Display )
    {
      m_AoeCircle.m_Color  = m_AoeColor;
    }
    else if ( res_event.m_Type == ResurrectionEvent.Type.Cast )
    {
      m_AoeCircle.m_Color  = Color.white;
      m_FadeTimeS          = res_event.m_TimeToDisplay;
      m_TimeSinceCastS     = 0f;
      m_Fading             = true;
    }
  }
}
