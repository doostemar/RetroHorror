using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitMovementSystem : MonoBehaviour
{
  UnitSelectionSystem m_UnitSelectionSystem;

  const string kMoveCommandButtonName = "Move Command";

  void Start()
  {
     m_UnitSelectionSystem = GetComponent<UnitSelectionSystem>();
  }

  void Update()
  {
    if ( Input.GetButtonUp( kMoveCommandButtonName ) )
    {
      List<SelectableUnit> selected_units = m_UnitSelectionSystem.SelectedUnits;

      // get centroid of all selected units
      List<Bot> moveable_units = new List<Bot>();
      Vector2 unit_centroid = Vector2.zero;
      int     unit_count    = 0;
      foreach ( SelectableUnit unit in selected_units )
      {
        Bot bot = unit.GetComponent<Bot>();
        if ( bot != null )
        {
          moveable_units.Add( bot );

          Vector2 unit_position = unit.transform.position;
          unit_centroid += unit_position;
          unit_count++;
        }
      }
      unit_centroid /= unit_count;

      // tell each unit to go to their offset from the click position
      Vector2 target_position = Camera.main.ScreenToWorldPoint( Input.mousePosition );
      foreach ( Bot unit in moveable_units )
      {
        Vector2 unit_position             = unit.transform.position;
        Vector2 unit_offset_from_centroid = unit_position - unit_centroid;
        Vector2 final_pos                 = target_position + unit_offset_from_centroid;

        BotMoveEvent move_event     = ScriptableObject.CreateInstance<BotMoveEvent>();
        move_event.m_TargetPosition = final_pos;
        move_event.m_Type           = BotMoveEvent.Type.Move;
        unit.Channel.RaiseMoveEvent( move_event );
      }
    }
  }
}
