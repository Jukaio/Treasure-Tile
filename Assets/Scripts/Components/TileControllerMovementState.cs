using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileControllerMovementState : StateMachineBehaviour
{
    protected TileController controller = null;
    protected Vector3 from = Vector3.zero;
    protected Vector3 offset = Vector3.up * 10.0f;
    protected Vector3 target = Vector3.zero;

    /* OnStateMachineEnter would make 100% sense more sense, but in this case Unity is being Unity...
       I think it is fixed in newer Unity versions: 
       "OnStateMachineEnter is called when entering a statemachine via its Entry Node" */
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // TODO: Change the way this is coupled - It's awkward; Break down components into smaller components
        // Idea: Target component -> any kind of target, etc.
        controller = animator.GetComponent<TileController>();
        offset = controller.Size;
        offset.x = 0.0f;
        offset.z = 0.0f;
        from = controller.transform.position;
        target = from + controller.Steps(controller.Direction);
        
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.position = target;
        controller.RefreshInWorld(from, target);
        controller.OnMoveFinish();
    }
}

