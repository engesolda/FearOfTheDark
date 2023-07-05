using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snaps : MonoBehaviour
{
    //Public
    public float damage;
    public float explosionSpeed;
    public float speedToExplode;

    //Private
    private bool snapExploded = false;
    private bool snapStopped = false;
    private bool canDamage = true;
    private Rigidbody2D myRB;
    private Light myLight;

    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light>();
        myRB = GetComponent<Rigidbody2D>();
        transform.position += new Vector3(0, 0, -1);

    }

    void FixedUpdate()
    {
        CheckForCollision();

        if (myRB.velocity.magnitude <= speedToExplode && !snapStopped)
        {
            snapStopped = true;
            GetComponent<AudioSource>().Play();
            myLight.range = 1.5f;
            myLight.intensity = 5;
        }

        if (snapStopped)
        {
            if (myLight.intensity >= 800)
            {
                snapExploded = true;
            }

            if (myLight.intensity <= 800 && !snapExploded)
            {   
                myLight.intensity += explosionSpeed;
            }

            if (myLight.intensity >= 0 && snapExploded)
            {
                myLight.intensity -= explosionSpeed;
            }

            if (myLight.intensity <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && !collision.CompareTag("Player") && !collision.CompareTag("Enemy") && !collision.CompareTag("Light") && collision.name.Contains("BlackHole"))
        {
            if (myRB)
            {
                myRB.velocity = Vector2.zero;
            }
        }
    }

    private void CheckForCollision()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(myLight.transform.position, myLight.range / 2, 1 << LayerMask.NameToLayer("Units"));

        foreach (Collider2D collision in collisions)
        {
            if (collision.CompareTag("Enemy") && canDamage && snapStopped)
            {
                collision.GetComponent<EnemyBase>().ApplyDamage(damage);
                canDamage = false;
            }
        }
    }
}
