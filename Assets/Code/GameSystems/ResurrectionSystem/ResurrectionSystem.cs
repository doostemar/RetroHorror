using System;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionSystem : MonoBehaviour
{
  [HideInInspector]
  public ResurrectionEventChannel m_ResChannel;
  public GameObject               m_SealSpritePrefab;

  [Flags]
  enum Flags
  {
    kFading        = 0x01,
    kCastThisFrame = 0x02
  }

  private HashSet< Collider2D > m_ResOverlaps;
  private GameObject            m_SealGameObj;
  private Collider2D            m_Collider;
  private Vector2               m_ResPosition;    // Cached from event
  private float                 m_FadeTimeS;      // Set from event
  private float                 m_TimeSinceCastS; // Set from event
  private int                   m_CorpseMask;
  private Flags                 m_Flags;

  private void Awake()
  {
    m_ResChannel = gameObject.AddComponent<ResurrectionEventChannel>();
    m_CorpseMask = LayerMask.GetMask(new string[] { "Corpse" });
  }

  void Start()
  {
    m_ResChannel.OnResurrectionEvent += OnResEvent;

    m_Flags       = 0;
    m_ResPosition = Vector2.zero;
    m_ResOverlaps = new HashSet< Collider2D >();
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
    if ( ( m_Flags & Flags.kFading ) > 0 )
    {
      m_TimeSinceCastS += Time.deltaTime;

      float fade_amt = 1f - ( m_TimeSinceCastS / m_FadeTimeS );
      {
        Color fade_cast = Color.white;
        fade_cast.a = fade_amt;
      }

      if ( m_TimeSinceCastS > m_FadeTimeS )
      {
        m_Flags &= ~Flags.kFading;
        Destroy( m_SealGameObj );
      }
    }
  }

  private void FixedUpdate()
  {
    if ( m_SealGameObj != null )
    {
      m_SealGameObj.transform.position = m_ResPosition;

      ContactFilter2D filter = new ContactFilter2D();
      filter.useLayerMask = true;
      filter.layerMask    = m_CorpseMask;
      List< Collider2D > overlaps = new List< Collider2D >();
      m_Collider.OverlapCollider( filter, overlaps );

      HashSet<Collider2D> exited_overlaps = new HashSet<Collider2D>( m_ResOverlaps );
      for ( int i_overlap = 0; i_overlap < overlaps.Count; ++i_overlap )
      {
        Collider2D      overlap = overlaps[ i_overlap ];
        CorpseComponent corpse  = overlap.GetComponent<CorpseComponent>();
        if ( exited_overlaps.Contains( overlap ) )
        {
          exited_overlaps.Remove( overlap );
        }
        else
        {
          m_ResOverlaps.Add( overlap );
          corpse.OnResEnter( );
        }

        if ( ( m_Flags & Flags.kCastThisFrame ) > 0 )
        {
          corpse.OnResCast( );
          m_ResOverlaps.Remove( overlap );
        }
      }

      m_Flags &= ~Flags.kCastThisFrame;

      foreach ( Collider2D exited_overlap in exited_overlaps )
      {
        CorpseComponent corpse = exited_overlap.GetComponent<CorpseComponent>();
        corpse.OnResExit();

        m_ResOverlaps.Remove( exited_overlap );
      }
    }
    else if ( m_ResOverlaps.Count > 0 )
    {
      m_ResOverlaps.Clear();
    }
  }

  void OnResEvent( ResurrectionEvent res_event )
  {
    m_ResPosition = res_event.m_Position;

    if ( res_event.m_Type == ResurrectionEvent.Type.Display )
    {
      if (m_SealGameObj == null)
      {
        m_SealGameObj = Instantiate(m_SealSpritePrefab, transform);
        m_SealGameObj.transform.position = res_event.m_Position;
        m_Collider    = m_SealGameObj.GetComponent<Collider2D>();
      }
    }
    else if ( res_event.m_Type == ResurrectionEvent.Type.Cast )
    {
      Animator anim = m_SealGameObj.GetComponent<Animator>();
      anim.Play( "ResSealAnimCast" );
      m_FadeTimeS      = res_event.m_TimeToDisplay;
      m_TimeSinceCastS = 0f;
      m_Flags         |= Flags.kFading | Flags.kCastThisFrame;
    }
  }
}
