using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{

  [HideInInspector]
  public  Collider2D    m_Collider;
  private SelectionRing m_Ring;

  private bool m_Selected;
  public bool Selected
  {
    get
    {
      return m_Selected;
    }
    set
    {
      if ( m_Ring != null )
      {
        m_Ring.enabled = value;
      }
      m_Selected = value;
    }
  }

  void Start()
  {
    m_Selected = false;
    m_Collider = GetComponent<Collider2D>();

    if ( m_Collider == null )
    {
      Debug.LogError( "Selectable item " + name + " did not have a Collider2D attached! Please attach one." );
    }

    m_Ring = GetComponent<SelectionRing>();

    if ( m_Ring == null ) 
    {
      Debug.LogError( "Selectable item " + name + " did not have a SelectionRing attached! Please attach one." );
    }
  }

  private void OnDestroy()
  {
    if ( m_Selected )
    {
      GameObject gamecon = Game.GetGameController();
      if ( gamecon != null )
      {
        UnitSelectionSystem sys = gamecon.GetComponent<UnitSelectionSystem>();
        sys.RemoveUnit( this );
      }
    }
  }
}
