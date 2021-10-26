using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Fighter
{
    public List<Transform> opponents;
    public Vector3 oppAvgPos;
    public Vector3 toOpponent;
    public bool engagingOpponent = false;

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
        HandleJump();
        IncrementTimers();
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
        base.ApplyRootRotation(lookDirection);
    }

    private void IncrementTimers()
    {
        timeSinceJumped += Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Punch");
            punchLookDir = toOpponent;
        }
    }

    private void HandleJump()
    {
        if (grounded && timeSinceJumped > 0.25f && Input.GetButtonDown("Jump"))
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