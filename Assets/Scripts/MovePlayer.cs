using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpUpPower = 2f; // cu bolta
    public float jumpPower = 2f;
    public Transform camera;
    private Rigidbody rigidbody;
    private Vector3 moveDir;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        float x = Input.GetAxis("Horizontal"); //1 pentru tasta D, -1 pentru tasta A, 0 altfel, x in [-1, 1] pentru gamepad
        float z = Input.GetAxis("Vertical"); //1 pentru tasta W, -1 pentru tasta S, 0 altfel, z in [-1, 1] pentru gamepad

        moveDir = (x * camera.right + z * camera.forward).normalized;
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized; // y e 0, miscare in xOz
                                                                          //transform.position += moveDir * Time.deltaTime * moveSpeed; // pentru non-rigidBody

        float velY = rigidbody.velocity.y;
        rigidbody.velocity = moveDir * moveSpeed;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY,
                                         rigidbody.velocity.z);

        if (Input.GetButtonDown("Jump"))
        {//saritura basic
            Vector3 jumpDir = (moveDir + Vector3.up * jumpUpPower).normalized;
            rigidbody.AddForce(jumpDir * jumpPower, ForceMode.VelocityChange);
        }
    }
}