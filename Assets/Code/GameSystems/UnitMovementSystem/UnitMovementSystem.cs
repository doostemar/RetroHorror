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
      List<BotChannel> moveable_units = new List<BotChannel>();
      Vector2 unit_centroid = Vector2.zero;
      int     unit_count    = 0;
      foreach ( SelectableUnit unit in selected_units )
      {
        BotChannel bot = unit.GetComponent<BotChannel>();
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
      Vector2 target_position = Game.GetRenderCamera().ScreenToWorldPoint( Input.mousePosition );
      foreach ( BotChannel unit in moveable_units )
      {
        Vector2 unit_position             = unit.transform.position;
        Vector2 unit_offset_from_centroid = unit_position - unit_centroid;
        Vector2 final_pos                 = target_position + unit_offset_from_centroid;

        BotMoveEvent move_event     = ScriptableObject.CreateInstance<BotMoveEvent>();
        move_event.m_TargetPosition = final_pos;
        move_event.m_Type           = BotMoveEvent.Type.Move;
        unit.RaiseMoveEvent( move_event );

        BotUnitChannel unit_channel = unit.gameObject.GetComponent<BotUnitChannel>();
        BotUnitEvent unit_evt = ScriptableObject.CreateInstance<BotUnitEvent>();
        unit_evt.m_Type       = BotUnitEvent.Type.MovementDirect;
        unit_evt.m_Position   = final_pos;
        unit_channel.RaiseUnitEvent( unit_evt );
      }
    }
  }
}
