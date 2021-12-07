using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Fighter
{
    public List<Transform> opponents;
    public Vector3 oppAvgPos;
    public Vector3 toOpponent;
    public bool engagingOpponent = false;
    public bool aiming = false;
    public bool isWallRunning = false;
    public Vector3 wallProjectedMoveDir;

    // Start is called before the first frame update
    private void Start()
    {
        GetCommonComponents();
    }

    private void OnEnable()
    {
        opponents = new List<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        GetMoveDirection();
        ApplyRootRotation();
        base.FighterUpdate();
        HandleAttack();
        HandleShooting();
        HandleWallRun();
        HandleJumpAndRoll();
        IncrementTimers();
    }

    private void HandleWallRun()
    {
        if (moveDir.magnitude < 10e-3f || !Input.GetButton("Jump"))
        {
            isWallRunning = false;
            animator.SetBool("WallRun", isWallRunning);
            return;//trebuie sa fugi spre zid si sa apesi pe jump daca vrei sa faci wallRun
        }

        Ray ray = new Ray();
        ray.origin = transform.position + Vector3.up * capsule.height / 2f;
        ray.direction = moveDir;

        int layerMask = ~LayerMask.NameToLayer("WallRunnable");
        float wallRunMaxDistance = 1.5f;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, wallRunMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            isWallRunning = true;
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * wallRunMaxDistance, Color.green);
			Debug.DrawLine(hitInfo.point, hitInfo.point + wallProjectedMoveDir, Color.cyan);
			wallProjectedMoveDir = Vector3.ProjectOnPlane(moveDir, hitInfo.normal);
            if (Input.GetButtonDown("Jump"))
            {
                float moveDirCrossWallDirY = Vector3.Cross(moveDir, wallProjectedMoveDir).y;

                animator.SetBool("WallRunSide", moveDirCrossWallDirY < 0f);
                StartCoroutine(StopWallRunning(4f));
            }
        }
        else
        {
            isWallRunning = false;
        }
        animator.SetBool("WallRun", isWallRunning);
    }
    IEnumerator StopWallRunning(float t) 
    {
        yield return new WaitForSeconds(t);
        isWallRunning = false;
        animator.SetBool("WallRun", isWallRunning);
    }
    private void HandleShooting()
    {
        aiming = Input.GetButton("Fire2");
        animator.SetBool("Aiming", aiming);
    }

    private void ApplyRootRotation()
    {
        Vector3 lookDirection = moveDir;
        toOpponent = lookDirection;
        float minDistanceToOpponent = float.PositiveInfinity;
        int numOpponentsInRange = 0;
        oppAvgPos = Vector3.zero;
        engagingOpponent = false;
        foreach (Transform opponent in opponents)
        {
            Vector3 toOpp = opponent.position - transform.position;
            float distToOpponent = toOpp.magnitude;
            if (distToOpponent < 4f)
            {
                if (distToOpponent < minDistanceToOpponent)
                {
                    minDistanceToOpponent = distToOpponent;
                    lookDirection = Vector3.ProjectOnPlane(toOpp, Vector3.up).normalized;
                    toOpponent = toOpp;
                    engagingOpponent = true;
                }
                oppAvgPos += opponent.position;
                numOpponentsInRange++;
            }
        }
        if (numOpponentsInRange > 0)
            oppAvgPos /= (float)numOpponentsInRange;
        else
            oppAvgPos = transform.position;
        animator.SetFloat("distToOpponent", minDistanceToOpponent);

        if (aiming)
            lookDirection = Vector3.ProjectOnPlane(camera.forward, Vector3.up).normalized;
        if (stateInfo.IsName("Roll"))
            lookDirection = moveDir;

        base.ApplyRootRotation(lookDirection);
    }

    private void IncrementTimers()
    {
        timeSinceJumped += Time.deltaTime;
    }

    private void HandleAttack()
    {
        animator.SetBool("Fire1Pressed", Input.GetButton("Fire1"));
        if (!aiming && Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Punch");
            punchLookDir = toOpponent;
        }
    }

    private void HandleJumpAndRoll()
    {
        if (!Input.GetButtonDown("Jump") || isWallRunning)
            return;

        if (engagingOpponent)
        {
            animator.SetTrigger("Roll");
            Vector3 rollDir = transform.forward;
            if (moveDir.magnitude > 10e-3f)
                rollDir = moveDir;
            animator.SetFloat("RollX", rollDir.x);
            animator.SetFloat("RollZ", rollDir.z);
        }
        else if (grounded && timeSinceJumped > 0.25f)
        {//saritura basic
            timeSinceJumped = 0f;
            animator.Play("Takeoff");
        }
    }

    private void GetMoveDirection()
    {
        float x = Input.GetAxis("Horizontal"); //1 pentru tasta D, -1 pentru tasta A, 0 altfel, x in [-1, 1] pentru gamepad
        float z = Input.GetAxis("Vertical"); //1 pentru tasta W, -1 pentru tasta S, 0 altfel, z in [-1, 1] pentru gamepad

        moveDir = (x * camera.right + z * camera.forward).normalized;
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized; // y e 0, miscare in xOz
                                                                          //transform.position += moveDir * Time.deltaTime * moveSpeed; // pentru non-rigidBody
    }
}