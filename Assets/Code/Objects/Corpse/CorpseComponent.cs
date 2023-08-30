using UnityEngine;

public class CorpseComponent : MonoBehaviour
{
  public GameObject m_ResurrectionSpawn;

  private BoxCollider2D m_Collider;
  private Animator      m_Animator;
  private bool          m_Highlighted;
  
  void Start()
  {
    GameObject game_controller = Game.GetGameController();
    ResurrectionEventChannel res_channel = game_controller.GetComponent<ResurrectionSystem>().m_ResChannel;
    res_channel.OnResurrectionEvent += OnResEvent;

    m_Collider = GetComponent<BoxCollider2D>();
    m_Animator = GetComponent<Animator>();
    m_Highlighted = false;
  }

  void OnResEvent( ResurrectionEvent res_event )
  {
    float dist_sq = m_Collider.bounds.SqrDistance( res_event.m_Position );
    if ( dist_sq < res_event.m_Radius * res_event.m_Radius )
    {
      if ( res_event.m_Type == ResurrectionEvent.Type.Display )
      {
        if ( m_Highlighted == false )
        {
          m_Animator.Play( "Highlighted" );
          m_Highlighted = true;
        }
        return;
      }
      else if ( res_event.m_Type == ResurrectionEvent.Type.Cast )
      {
        Instantiate( m_ResurrectionSpawn, transform.position, Quaternion.identity );
        Destroy( gameObject );
      }
    }

    if ( m_Highlighted )
    {
      m_Animator.Play( "Corpse" );
      m_Highlighted = false;
    }
  }
}
