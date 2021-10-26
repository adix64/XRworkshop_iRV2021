using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    private float yaw, pitch;
    public Transform target;
    private Player player;
    public Vector3 cameraOffset;
    public float followSpeed = 3f;
    public float followSpeedKeepClose = 3f;
    public float zeroingSpeed = 10f;
    public Transform targetVisual;
    private float xOffsetZeroing = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        player = target.GetComponent<Player>();
    }

    // Update is called once per frame
    private void LateUpdate() //target position e calculat in Update, iar camera trebuie calculata dupa
    {
        pitch -= Input.GetAxis("Mouse Y");
        yaw += Input.GetAxis("Mouse X");

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        targetVisual.position = Vector3.Lerp(target.position, player.oppAvgPos, 0.5f);

        Vector3 realCameraOffset = cameraOffset;
        if (player.moveDir.magnitude > 10e-3f || player.engagingOpponent)
            xOffsetZeroing = Mathf.Lerp(xOffsetZeroing, 1f, Mathf.Clamp01(Time.deltaTime * zeroingSpeed));
        else
            xOffsetZeroing = Mathf.Lerp(xOffsetZeroing, 0f, Mathf.Clamp01(Time.deltaTime * zeroingSpeed));

        realCameraOffset.x = Mathf.Lerp(cameraOffset.x, 0f, xOffsetZeroing);
        Vector3 newCameraPosition = targetVisual.position + transform.TransformDirection(realCameraOffset);
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