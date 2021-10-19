using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Fighter
{
    // Start is called before the first frame update
    private void Start()
    {
        GetCommonComponents();
    }

    // Update is called once per frame
    private void Update()
    {
        GetMoveDirection();
        base.FighterUpdate();
        HandleAttack();
        HandleJump();
        IncrementTimers();
    }

    private void IncrementTimers()
    {
        timeSinceJumped += Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (Input.GetButton("Fire1"))
            animator.SetTrigger("Punch");
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