using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpUpPower = 2f; // cu bolta
    public float jumpPower = 2f;
    public Transform camera;
    public Rigidbody rigidbody;
    public Vector3 moveDir;
    protected Vector3 initialPos;
    public float rotSpeed = 3f;
    protected Animator animator;
    public float minimumAllowedHeight = -50f;
    public float groundedThreshold = 0.15f;
    protected CapsuleCollider capsule;
    protected AnimatorStateInfo stateInfo;
    protected bool grounded = true;
    protected float timeSinceJumped = 10;

    // Start is called before the first frame update
    protected void GetCommonComponents()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        initialPos = transform.position;
    }

    // Update is called once per frame
    protected void FighterUpdate()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        CheckGrounded();

        SetAnimatorMoveParams();

        ApplyRootRotation();
    }

    private void OnAnimatorMove()
    {
        ApplyRootMotion();
    }

    private void ApplyRootRotation()
    {
        if (stateInfo.IsTag("punch"))
            return;
        if (transform.position.y < minimumAllowedHeight)
            transform.position = initialPos;
        if (moveDir.magnitude > 10e-3)
        {//daca exista miscare
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            //roteste personajul animativ cu forward in directia deplasarii
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                    targetRotation,
                                                    Time.deltaTime * rotSpeed);
        }
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray();
        ray.direction = Vector3.down;
        Vector3 rayOrigin = transform.position + groundedThreshold * Vector3.up;
        grounded = false;
        for (float xOffset = -1f; xOffset <= 1f; xOffset += 1f)
        {
            for (float zOffset = -1f; zOffset <= 1f; zOffset += 1f)
            {
                Vector3 offset = new Vector3(xOffset, 0f, zOffset).normalized * capsule.radius;
                ray.origin = rayOrigin + offset;
                if (Physics.Raycast(ray, 2f * groundedThreshold))
                {
                    grounded = true;
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f * groundedThreshold, Color.green);
                }
                else
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f * groundedThreshold, Color.red);
            }
        }
        animator.SetBool("Grounded", grounded);
    }

    private void ApplyRootMotion()
    {
        if (!animator.GetBool("Grounded") || timeSinceJumped < 0.25f)//daca e in aer
            return;

        float velY = rigidbody.velocity.y;
        rigidbody.velocity = animator.deltaPosition / Time.deltaTime;// moveDir * moveSpeed;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY,
                                         rigidbody.velocity.z);
    }

    private void SetAnimatorMoveParams()
    {
        var characterSpaceMoveDir = transform.InverseTransformDirection(moveDir);
        animator.SetFloat("Forward", characterSpaceMoveDir.z, 0.2f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceMoveDir.x, 0.2f, Time.deltaTime);

        Debug.DrawLine(transform.position, transform.position + moveDir, Color.cyan);
    }
}