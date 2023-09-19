using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionSystem : MonoBehaviour
{
  enum State
  {
    None,
    PotentiallyClicking,
    Selecting
  }

  public float      m_ClickTimerSeconds;
  public float      m_ClickMousePositionDelta;
  public GameObject m_SelectionLineRendererPrefab;
  
  private List<SelectableUnit> m_SelectedUnits;
  public  List<SelectableUnit> SelectedUnits
  {
    get { return m_SelectedUnits; }
  }

  private List<SelectableUnit> m_SelectingUnits;
  private Vector2              m_DownMousePos;
  private State                m_State;
  private float                m_TimeSinceDownSeconds;
  private int                  m_UnitsMask;
  private bool                 m_AddHeldForAction;
  private LineRenderer         m_LineRenderer;

  const string kSelectButtonName    = "Select";
  const string kSelectAddButtonName = "SelectAdd";

  private void Start()
  {
    GameObject line_renderer_obj = Instantiate( m_SelectionLineRendererPrefab, transform );
    m_LineRenderer               = line_renderer_obj.GetComponent<LineRenderer>();
    m_LineRenderer.enabled       = false;
    
    m_SelectedUnits              = new List<SelectableUnit>();
    m_SelectingUnits             = new List<SelectableUnit>();
    
    m_State                      = State.None;
    m_UnitsMask                  = LayerMask.GetMask( new string[]{ "HeroUnit" } );
  }

  private void Update()
  {
    bool add_held = Input.GetButton( kSelectAddButtonName );
    if ( add_held == false )
    {
      m_AddHeldForAction = false;
    }

    switch ( m_State )
    {
      case State.None:
        HandleNoneState( add_held );
        break;
      case State.PotentiallyClicking:
        HandlePotentiallyClicking( );
        break;
      case State.Selecting:
        HandleSelecting();
        break;
    }
  }

  void HandleNoneState( bool add_held )
  {
    if ( Input.GetButtonDown( kSelectButtonName ) )
    {
      m_TimeSinceDownSeconds = 0f;
      m_DownMousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
      m_State = State.PotentiallyClicking;
      m_AddHeldForAction = add_held;

      if ( m_AddHeldForAction == false )
      {
        ClearUnits( m_SelectedUnits );
      }
    }
  }

  void HandlePotentiallyClicking( )
  {
    if ( Input.GetButtonUp( kSelectButtonName ) )
    {
      HandleClick( );
      return;
    }

    m_TimeSinceDownSeconds += Time.deltaTime;

    Vector2 mouse_down_now                      = Camera.main.ScreenToWorldPoint( Input.mousePosition );
    bool    mouse_movement_over_click_threshold = (mouse_down_now - m_DownMousePos).sqrMagnitude 
                                                > m_ClickMousePositionDelta * m_ClickMousePositionDelta;

    if ( m_TimeSinceDownSeconds > m_ClickTimerSeconds || mouse_movement_over_click_threshold )
    {
      m_State = State.Selecting;
    }
  }

  private void HandleClick( )
  {
    if ( m_AddHeldForAction == false )
    {
      ClearUnits( m_SelectedUnits );
    }

    m_State = State.None;

    RaycastHit2D cast_result = Physics2D.GetRayIntersection( Camera.main.ScreenPointToRay( Input.mousePosition ), 100f, m_UnitsMask );
    if ( cast_result.collider != null )
    {
      GameObject hit_obj = cast_result.transform.gameObject;
      SelectableUnit unit = hit_obj.GetComponent<SelectableUnit>();
      if ( unit != null )
      {
        m_SelectedUnits.Add( unit );
        unit.Selected = true;
      }
      else
      {
        GameObject err_obj = cast_result.collider.gameObject;
        Transform  parent  = err_obj.transform.parent;
        Debug.LogError( "Clicked object on selectable layer that does not have SelectableUnit object: " 
                        + cast_result.collider.gameObject.name
                        + ( parent == null ? "" : ", " + parent.name ) );
      }
    }
  }

  private void HandleSelecting()
  {
    if ( Input.GetButtonUp( kSelectButtonName ) )
    {
      HandleAreaSelect( );
      return;
    }

    Vector2 mouse_world_pos = Camera.main.ScreenToWorldPoint( Input.mousePosition );

    // do the actual operation of selection
    {
      Collider2D[]         colliders = Physics2D.OverlapAreaAll( m_DownMousePos, mouse_world_pos, m_UnitsMask );
      List<SelectableUnit> sel_tmp   = new List<SelectableUnit>();
      
      foreach ( Collider2D collider in colliders )
      {
        GameObject hit_obj  = collider.transform.gameObject;
        SelectableUnit unit = hit_obj.GetComponent<SelectableUnit>();
        if ( unit != null )
        {
          sel_tmp.Add( unit );
          unit.Selected = true;
        }
      }
      
      // fancy c#. we're web developers now
      List<SelectableUnit> unselected_units = m_SelectingUnits.Where( u => !sel_tmp.Any( u2 => u2 == u ) ).ToList();
      foreach ( SelectableUnit unit in unselected_units )
      {
        unit.Selected = false;
      }
      
      m_SelectingUnits = sel_tmp;
    }

    // draw the box
    {
      m_LineRenderer.enabled = true;
      
      Vector3[] sel_pts =
      {
        new Vector3( m_DownMousePos.x,  m_DownMousePos.y  ),
        new Vector3( m_DownMousePos.x,  mouse_world_pos.y ),
        new Vector3( mouse_world_pos.x, mouse_world_pos.y ),
        new Vector3( mouse_world_pos.x, m_DownMousePos.y  )
      };
      
      m_LineRenderer.positionCount = sel_pts.Length;
      m_LineRenderer.SetPositions( sel_pts );
    }
  }

  private void HandleAreaSelect()
  {
    if ( m_AddHeldForAction == false )
    {
      ClearUnits( m_SelectedUnits );
    }

    m_State = State.None;
    m_LineRenderer.enabled = false;
    m_SelectedUnits.AddRange( m_SelectingUnits );
    m_SelectingUnits.Clear();
  }

  private void ClearUnits( List<SelectableUnit> units )
  {
    foreach ( SelectableUnit unit in units )
    {
      unit.Selected = false;
    }
    units.Clear();
  }

  public void RemoveUnit( SelectableUnit unit )
  {
    m_SelectedUnits.Remove( unit );
  }
}
