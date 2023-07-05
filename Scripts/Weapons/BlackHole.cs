using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    //Public
    public float dragForce;
    public float distanceMultiplier;
    public float lifeTime;
    public float speed;
    public int damage;

    //Private
    private Animator myAnimator;
    private bool dead = false;
    private bool stopped = false;
    private Vector2 destinationPoint = Vector2.zero;
    private Vector2 newPos = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        var emission = GetComponent<ParticleSystem>().emission;
        emission.enabled = false;

        Invoke("Die", lifeTime);
    }

    private void Update()
    {
        CheckCollisions();
        StartParticles();

        newPos = Vector2.MoveTowards(transform.position, destinationPoint, Time.fixedDeltaTime * speed);
        GetComponent<Rigidbody2D>().MovePosition(newPos);

        if (newPos == (Vector2)transform.position)
        {
            stopped = true;
            myAnimator.SetTrigger("CanPlay");
        }
    }

    private void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;
    }

    public void SetDestinationPoint(Vector2 _dest)
    {
        destinationPoint = _dest;
    }

    private void StartParticles()
    {
        if (stopped)
        {
            var emission = GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
        }
    }

    public bool IsStopped()
    {
        return stopped;
    }

    public void Die()
    {
        dead = true;
        myAnimator.SetTrigger("Die");
        Destroy(gameObject, myAnimator.runtimeAnimatorController.animationClips[2].length);
    }

    private void CheckCollisions()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().bounds.size.x / 2);

        foreach (Collider2D collision in collisions)
        {
            if (collision.IsTouching(GetComponent<PolygonCollider2D>()) && !dead && stopped)
            {
                if (collision.CompareTag("Player"))
                {//If the player hits the center of the black hole

                    Player.GetInstance().ApplyDamageToPlayer(damage);
                    myAnimator.SetTrigger("Die");
                    var emission = GetComponent<ParticleSystem>().emission;
                    emission.enabled = false;
                    Destroy(gameObject, myAnimator.runtimeAnimatorController.animationClips[2].length);
                    dead = true;
                }
            }

            if ((collision.CompareTag("Player") || collision.name.Contains("Snaps")) && stopped)
            {//If the player if inside the gravity force of the black hole

                //The force will grow as the player gets closer to the center
                collision.GetComponent<Rigidbody2D>().AddForce((transform.position - collision.transform.position).normalized * (distanceMultiplier - Vector2.Distance(transform.position, collision.transform.position)));
            }

            if (collision.gameObject.name == "BlackHole" && collision.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {//If two black hole want to live in the same area the yonger will die =/

                if (stopped && collision.GetComponent<BlackHole>().IsStopped() && !dead)
                {
                    if (lifeTime < collision.GetComponent<BlackHole>().lifeTime)
                    {
                        collision.GetComponent<BlackHole>().Die();
                    }
                }
            }
        }
    }
}
