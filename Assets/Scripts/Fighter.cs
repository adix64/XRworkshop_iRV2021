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
    protected Vector3 punchLookDir;

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
    }

    private void OnAnimatorMove()
    {
        ApplyRootMotion();
    }

    protected void ApplyRootRotation(Vector3 lookDir)
    {
        float realRotSpeed = rotSpeed;
        if (stateInfo.IsTag("punch"))
        {
            lookDir = punchLookDir;
            realRotSpeed *= 10f;
        }
        if (transform.position.y < minimumAllowedHeight)
            transform.position = initialPos;
        if (moveDir.magnitude > 10e-3)
        {//daca exista miscare
            Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            //roteste personajul animativ cu forward in directia deplasarii
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                    targetRotation,
                                                    Mathf.Clamp01(Time.deltaTime * realRotSpeed));
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

    protected void ApplyRootMotion()
    {
        float velY = rigidbody.velocity.y; //componenta verticala a vitezei, calculata de Physics engine
        if (!animator.GetBool("Grounded") || timeSinceJumped < 0.25f)//in aer
        {
            float jumpDirControlBlendF = Mathf.Pow(Mathf.Clamp01(timeSinceJumped * 0.5f), 8f);
            rigidbody.velocity = Vector3.Lerp(moveDir * moveSpeed, rigidbody.velocity, jumpDirControlBlendF);
        }
        else//pe pamant
        {
            float rootMotionMoveMagnitude = (animator.deltaPosition / Time.deltaTime).magnitude;
            rigidbody.velocity = moveDir * rootMotionMoveMagnitude;// moveDir * moveSpeed;
        }

        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY,
                                         rigidbody.velocity.z);
    }

    private void SetAnimatorMoveParams()
    {//facem directia de deplasare mai mare ca sa asigure full range in blend tree (e.g. de 1.2 ori mai mare):
        var characterSpaceMoveDir = transform.InverseTransformDirection(moveDir) * 1.2f;

        animator.SetFloat("Forward", characterSpaceMoveDir.z, 0.2f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceMoveDir.x, 0.2f, Time.deltaTime);

        Debug.DrawLine(transform.position, transform.position + moveDir, Color.cyan);
    }
}