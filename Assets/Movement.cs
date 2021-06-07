using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Movement : StateMachineBehaviour
{
    PlayerController controller = null;
    private Vector3 from = Vector3.zero;
    private Vector3 offset = Vector3.up * 10.0f;
    private Vector3 target = Vector3.zero;

    /* OnStateMachineEnter would make 100% sense more sense, but in this case Unity is being Unity...
       I think it is fixed in newer Unity versions: 
       "OnStateMachineEnter is called when entering a statemachine via its Entry Node" */
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller = animator.GetComponent<PlayerController>();
        from = controller.transform.position;
        target = from + controller.Steps(controller.Direction);
        
    }

    Vector3 SinEasing(Vector3 that, float t)
    {
        var factor = Mathf.Sin(t * (Mathf.PI * 0.5f));
        return that * factor;
    }
    Vector3 SinEasingBounce(Vector3 that, float t)
    {
        var factor = Mathf.Sin(t * Mathf.PI);
        return that * factor;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var jump = SinEasingBounce(offset, stateInfo.normalizedTime);
        var current = SinEasing(target - from + jump, stateInfo.normalizedTime);
        animator.gameObject.transform.position = from + current;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.position = target;
        controller.RefreshPlayerInWorld(from, target);
        controller.PlayDust();
        // Particles
        // Sound
    }
}
