using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal enum TargetOffsetMode
{ TARGET_PLAYER, CIRCLE_LEFT, CIRCLE_RIGHT, RETREAT, NUM_MODES };

public class Opponent : Fighter
{
    private UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    public float stopDistance = 0.9f;
    public float attackDistance = 1f;

    [Range(0f, 1f)]
    public float offensiveRate = 0.2f;

    public bool offensive = false;
    private TargetOffsetMode targetOffsetMode = TargetOffsetMode.TARGET_PLAYER;
    private Vector3 offsetFromPlayer;
    public float offsetFromPlayerMagnitude = 2f;

    private void OnEnable()
    {
        GameObject.FindObjectOfType<Player>().opponents.Add(transform);
    }

    private void OnDestroy()
    {
        GameObject.FindObjectOfType<Player>().opponents.Remove(transform);
    }

    // Start is called before the first frame update
    private void Start()
    {
        GetCommonComponents();
        StartCoroutine(SetOffensiveState());
        StartCoroutine(SetTargetOffsetState());
    }

    private IEnumerator SetTargetOffsetState()//will circle around player
    {
        yield return new WaitForSeconds(1f);
        targetOffsetMode = (TargetOffsetMode)Random.Range(0, (int)TargetOffsetMode.NUM_MODES);
        switch (targetOffsetMode)
        {
            case TargetOffsetMode.TARGET_PLAYER:
                offsetFromPlayer = Vector3.zero;
                break;

            case TargetOffsetMode.CIRCLE_LEFT:
                offsetFromPlayer = -transform.right * offsetFromPlayerMagnitude;
                break;

            case TargetOffsetMode.CIRCLE_RIGHT:
                offsetFromPlayer = transform.right * offsetFromPlayerMagnitude;
                break;

            case TargetOffsetMode.RETREAT:
                offsetFromPlayer = -transform.forward * offsetFromPlayerMagnitude;
                break;
        }
        yield return StartCoroutine(SetTargetOffsetState());
    }

    private IEnumerator SetOffensiveState()
    {
        yield return new WaitForSeconds(0.5f);
        offensive = Random.Range(0f, 1f) < offensiveRate;
        yield return StartCoroutine(SetOffensiveState());
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 toPlayer = (player.position - transform.position);
        toPlayer = Vector3.Scale(toPlayer, new Vector3(1, 0, 1));

        if (offensive && toPlayer.magnitude < attackDistance)
            animator.SetTrigger("Punch");

        if (toPlayer.magnitude < stopDistance)
            toPlayer = Vector3.zero;
        animator.SetFloat("distToOpponent", toPlayer.magnitude);
        moveDir = toPlayer.normalized;
        ApplyRootRotation(moveDir);
        moveDir = (toPlayer + offsetFromPlayer).normalized;
        base.FighterUpdate();
    }
}