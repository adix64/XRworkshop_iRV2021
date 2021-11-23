using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterMechanics : MonoBehaviour
{
    public Transform weapon;
    public Transform weaponTip;
    private Animator animator;
    private Transform rightHand;
    private Player player;
    private Transform chest, upperchest, head;
    
    GameObject[] projectiles;
    public Transform projectilesContainer;
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        upperchest = animator.GetBoneTransform(HumanBodyBones.UpperChest);
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        projectiles = new GameObject[projectilesContainer.childCount];
        for (int i = 0; i < projectilesContainer.childCount; i++)
            projectiles[i] = projectilesContainer.GetChild(i).gameObject;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        weapon.gameObject.SetActive(player.aiming);
        if (!player.aiming)
            return;

        CopyRightHandTransformToWeapon();
        Quaternion tipToAimRot = Quaternion.FromToRotation(weaponTip.right, player.camera.forward);
        tipToAimRot.ToAngleAxis(out float angle, out Vector3 axis);
        chest.rotation = Quaternion.AngleAxis(angle * 0.5f, axis) * chest.rotation;
        upperchest.rotation = Quaternion.AngleAxis(angle * 0.5f, axis) * upperchest.rotation;
        CopyRightHandTransformToWeapon();
        head.rotation = player.camera.rotation;
        if (Input.GetButtonDown("Fire1"))
        {
            for (int i = 0; i < projectilesContainer.childCount; i++)
            {
                if (!projectiles[i].activeInHierarchy)
                {
                    projectiles[i].SetActive(true);
                    projectiles[i].transform.position = weaponTip.position;
                    projectiles[i].transform.rotation = weaponTip.rotation;
                    break;
                }
            }
        }
    }

    private void CopyRightHandTransformToWeapon()
    {
        weapon.position = rightHand.position;
        weapon.rotation = rightHand.rotation;
    }
}