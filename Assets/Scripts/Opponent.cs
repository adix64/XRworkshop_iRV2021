using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : Fighter
{
    private UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    public float stopDistance = 0.9f;
    public float attackDistance = 1f;

    [Range(0f, 1f)]
    public float offensiveRate = 0.2f;

    public bool offensive = false;

    private void OnEnable()
    {
        GameObject.FindObjectOfType<Player>().opponents.Add(transform);
    }

    // Start is called before the first frame update
    private void Start()
    {
        GetCommonComponents();
        StartCoroutine(SetOffensiveState());
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
        base.FighterUpdate();
    }
}