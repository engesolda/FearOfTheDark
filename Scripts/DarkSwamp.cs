using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSwamp : MonoBehaviour
{
    //Public
    public int initialDamage;
    public int dotDamage;
    public int dotTimes;
    public float timeBwDots;

    //Private
    [HideInInspector]public bool alreadyAppliedPoison = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Player") && !alreadyAppliedPoison)
        {
            collision.gameObject.GetComponent<Player>().ApplyDamageToPlayer(initialDamage);
            collision.gameObject.GetComponent<Player>().ApplyDotToPlayer(dotDamage, dotTimes, timeBwDots);
            alreadyAppliedPoison = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Equals("Player") && !alreadyAppliedPoison)
        {
            collision.gameObject.GetComponent<Player>().ApplyDamageToPlayer(initialDamage);
            collision.gameObject.GetComponent<Player>().ApplyDotToPlayer(dotDamage, dotTimes, timeBwDots);
            alreadyAppliedPoison = true;
        }
    }
}
