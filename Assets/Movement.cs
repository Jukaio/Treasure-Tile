using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Movement : TileControllerMovementState
{
    /* OnStateMachineEnter would make 100% sense more sense, but in this case Unity is being Unity...
       I think it is fixed in newer Unity versions: 
       "OnStateMachineEnter is called when entering a statemachine via its Entry Node" */
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var jump = offset.EasingSinInBounce(stateInfo.normalizedTime);
        var current = (target - from).EasingSinIn(stateInfo.normalizedTime) + jump;
        animator.gameObject.transform.position = from + current;
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        controller.RequestMove(target);
    }

}
