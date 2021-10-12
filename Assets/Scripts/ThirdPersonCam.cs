using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    private float yaw, pitch;
    public Transform target;
    public Vector3 cameraOffset;
    public float followSpeed = 3f;
    public float followSpeedKeepClose = 3f;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void LateUpdate() //target position e calculat in Update, iar camera trebuie calculata dupa
    {
        pitch -= Input.GetAxis("Mouse Y");
        yaw += Input.GetAxis("Mouse X");

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 newCameraPosition = target.position + transform.TransformDirection(cameraOffset);
        //lazy camera interpolation:

        Vector3 cameraMoveDir = newCameraPosition - transform.position;
        if (cameraMoveDir.magnitude > 10e-3f)
        {
            transform.position += cameraMoveDir.normalized *
                                Mathf.Min(cameraMoveDir.magnitude, Time.deltaTime * followSpeed *
                                (1 + cameraMoveDir.magnitude * followSpeedKeepClose));
        }
    }
}