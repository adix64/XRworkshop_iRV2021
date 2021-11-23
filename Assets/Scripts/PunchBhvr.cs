using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBhvr : StateMachineBehaviour
{
    public float hitBoxStartT = 0.035f;
    public float hitBoxEndT = 0.2f;
    public HumanBodyBones bone;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var boneTransform = animator.GetBoneTransform(bone);
        var hitBox = boneTransform.GetComponent<HitBox>();
        hitBox.dirty = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var boneTransform = animator.GetBoneTransform(bone);
        var collider = boneTransform.GetComponent<Collider>();
        var hitBox = boneTransform.GetComponent<HitBox>();
		collider.enabled = stateInfo.normalizedTime > hitBoxStartT &&
			stateInfo.normalizedTime < hitBoxEndT && !hitBox.dirty;

        animator.speed = animator.GetBool("Fire1Pressed") &&
            stateInfo.normalizedTime < hitBoxStartT ? 0.3f : 1f;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var boneTransform = animator.GetBoneTransform(bone);
        var collider = boneTransform.GetComponent<Collider>();
        collider.enabled = false;
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