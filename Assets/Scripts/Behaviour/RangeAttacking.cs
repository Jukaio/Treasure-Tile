using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttacking : TileControllerMovementState
{
    // TODO: replace the behaviour of this class with some sort of projectile behaviour
    // It is range though, but instead of "shooting itself" it should shoot a projectile

    private bool IsForward = true;
    private RangeEnemyController range_controller = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        range_controller = animator.GetComponent<RangeEnemyController>();

        target = range_controller.Target;
        IsForward = true;
    }

    void GoBackward()
    {
        IsForward = false;
        controller.OnAttack(target);
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
        controller.OnAttackFinish();
    }
}
