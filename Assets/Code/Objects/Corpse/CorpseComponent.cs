using UnityEngine;

public class CorpseComponent : MonoBehaviour
{
  public GameObject m_ResurrectionSpawn;

  private BoxCollider2D m_Collider;
  private Animator      m_Animator;
  private bool          m_Highlighted;

  void Start()
  {
    m_Collider = GetComponent<BoxCollider2D>();
    m_Animator = GetComponent<Animator>();
    m_Highlighted = false;
  }

  private void Update()
  {
    if ( m_Highlighted )
    {
      m_Animator.Play( "Corpse" );
      m_Highlighted = false;
    }
  }

  public void OnResEnter()
  {
    if ( m_Highlighted == false )
    {
      m_Animator.Play( "CorpseHighlightedShader" );
      m_Highlighted = true;
    }
  }

  public void OnResExit()
  {
    m_Animator.Play("Corpse");
    m_Highlighted = false;
  }

  public void OnResCast( )
  {
    GameObject resed_obj = Instantiate( m_ResurrectionSpawn, transform.position, Quaternion.identity );
    CreatedFromCorpseComponent cfcc = resed_obj.AddComponent<CreatedFromCorpseComponent>();
    Destroy( gameObject );
  }
}
