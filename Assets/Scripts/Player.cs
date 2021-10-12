using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpUpPower = 2f; // cu bolta
    public float jumpPower = 2f;
    public Transform camera;
    public Rigidbody rigidbody;
    public Vector3 moveDir;
    private Vector3 initialPos;
    public float rotSpeed = 3f;
    private Animator animator;
    public float minimumAllowedHeight = -50f;
    public float groundedThreshold = 0.15f;
    public float timeSinceJumped = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        initialPos = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        GetMoveDirection();

        ApplyRootRotation();

        HandleJump();

        IncrementTimers();
    }

    private void OnAnimatorMove()
    {
        ApplyRootMotion();
    }

    private void IncrementTimers()
    {
        timeSinceJumped += Time.deltaTime;
    }

    private void ApplyRootRotation()
    {
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

    private void HandleJump()
    {
        Ray ray = new Ray();
        ray.direction = Vector3.down;
        ray.origin = transform.position + groundedThreshold * Vector3.up;
        if (Physics.SphereCast(ray, 2f * groundedThreshold))
        {
            animator.SetBool("Grounded", true);

            if (timeSinceJumped > 0.25f && Input.GetButtonDown("Jump"))
            {//saritura basic
                timeSinceJumped = 0f;
                animator.Play("Takeoff");
            }
        }
        else animator.SetBool("Grounded", false);
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

    private void GetMoveDirection()
    {
        float x = Input.GetAxis("Horizontal"); //1 pentru tasta D, -1 pentru tasta A, 0 altfel, x in [-1, 1] pentru gamepad
        float z = Input.GetAxis("Vertical"); //1 pentru tasta W, -1 pentru tasta S, 0 altfel, z in [-1, 1] pentru gamepad

        moveDir = (x * camera.right + z * camera.forward).normalized;
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized; // y e 0, miscare in xOz
                                                                          //transform.position += moveDir * Time.deltaTime * moveSpeed; // pentru non-rigidBody

        var characterSpaceMoveDir = transform.InverseTransformDirection(moveDir);
        animator.SetFloat("Forward", characterSpaceMoveDir.z, 0.2f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceMoveDir.x, 0.2f, Time.deltaTime);
    }
}