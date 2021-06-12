using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Movement : TileControllerMovementState
{
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
