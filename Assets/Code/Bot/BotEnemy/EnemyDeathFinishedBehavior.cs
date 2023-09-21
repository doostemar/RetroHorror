using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathFinishedBehavior : StateMachineBehaviour
{
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    BotEnemyChannel enemy_channel = animator.GetComponent<BotEnemyChannel>();
    BotEnemyEvent evt             = CreateInstance<BotEnemyEvent>();
    evt.m_Type = BotEnemyEvent.Type.DeathAnimFinished;
    enemy_channel.RaiseEnemyEvent( evt );
  }
}
