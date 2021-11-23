using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float timeLived = 0f;
    const float lifetime = 5f;
    public float speed = 64f;
    public int damage = 10;
    TrailRenderer trailRenderer;
	private void Start()
	{
        trailRenderer = GetComponent<TrailRenderer>();
	}

	private void OnEnable()
	{
        timeLived = 0f;
	}

	// Update is called once per frame
	void Update()
    {
        timeLived += Time.deltaTime;
        if (timeLived > lifetime)
        {
            trailRenderer.emitting = false;
            gameObject.SetActive(false) ;
            return;
        }

        transform.position += speed * transform.right * Time.deltaTime;
        trailRenderer.emitting = true;
    }
	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyAnimator = other.GetComponent<Animator>();
            enemyAnimator.SetInteger("takenDamage", damage);
            enemyAnimator.Play(Random.Range(0,2) == 0 ?
                                    "TakeHit.takeHitL" :
                                    "TakeHit.takeHitR");
            trailRenderer.emitting = false;
            gameObject.SetActive(false) ;
        }
	}
}
