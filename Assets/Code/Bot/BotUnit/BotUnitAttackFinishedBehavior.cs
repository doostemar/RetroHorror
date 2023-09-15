using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnitAttackFinishedBehavior : StateMachineBehaviour
{
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    BotUnitChannel unit_channel = animator.GetComponent<BotUnitChannel>();
    BotUnitEvent evt = ScriptableObject.CreateInstance<BotUnitEvent>();
    evt.m_Type = BotUnitEvent.Type.AttackFinished;
    unit_channel.RaiseUnitEvent( evt );
  }
}
