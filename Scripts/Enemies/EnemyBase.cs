using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //Public
    public bool dead = false;    

    //Private

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ApplyDamage(float _amount)
    {
    }
}
