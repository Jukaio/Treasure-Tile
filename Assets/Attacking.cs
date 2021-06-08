using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : TileControllerMovementState
{
    bool IsForward = true;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        IsForward = true;
    }

    void GoBackward()
    {
        IsForward = false;
        controller.Attack(target);
        controller.OnMoveFinish();
        // Stuff
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(IsForward) {
            var jump = offset.EasingSinInBounce(stateInfo.normalizedTime * 2.0f);
            var current = (target - from).EasingSinInCubic(stateInfo.normalizedTime * 2.0f) + jump;
            animator.gameObject.transform.position = from + current;
            if(stateInfo.normalizedTime >= 0.5f) {
                GoBackward();
            }
        }
        else { 
            var jump = offset.EasingSinInBounce((stateInfo.normalizedTime - 0.5f) * 2.0f);
            var current = (from - target).EasingSinOutCubic((stateInfo.normalizedTime - 0.5f) * 2.0f) + jump;
            animator.gameObject.transform.position = target + current;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.position = from;
        controller.OnMoveFinish();
    }
}
