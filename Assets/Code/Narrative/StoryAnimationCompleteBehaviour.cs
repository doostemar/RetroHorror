using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryAnimationCompleteBehaviour : StateMachineBehaviour
{
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    GameObject storyteller = GameObject.Find( "Storyteller" );
    StorySequence story = storyteller.GetComponent<StorySequence>();
    story.AnimationFinished();
  }

}
