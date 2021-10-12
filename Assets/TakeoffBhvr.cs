using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoffBhvr : StateMachineBehaviour
{
    private Player player;
    private bool hasToTakeOff = false;

    private void OnEnable()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasToTakeOff = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.75 && hasToTakeOff)
            DoJump();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hasToTakeOff)
            DoJump();
    }

    private void DoJump()
    {
        Vector3 jumpDir = (player.moveDir + Vector3.up * player.jumpUpPower).normalized;
        player.rigidbody.AddForce(jumpDir * player.jumpPower, ForceMode.VelocityChange);
        hasToTakeOff = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}