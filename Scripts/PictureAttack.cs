using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAttack : MonoBehaviour
{
    //Public
    public float swampLifeTime;

    //Private
    private GameObject mySpit;
    private Animator mySpitAnimator;
    //private Animator swampAnimator;
    private GameObject mySwamp;
    private float swampDefaultLifeTime;


    // Start is called before the first frame update
    void Start()
    {
        mySpit = transform.GetChild(0).gameObject;
        mySpitAnimator = mySpit.GetComponent<Animator>();

        mySwamp = transform.GetChild(1).gameObject;
        //swampAnimator = mySwamp.GetComponent<Animator>();
        swampDefaultLifeTime = swampLifeTime;
    }

    private void Update()
    {
        /*if (swampAnimator.GetBool("Live"))
        {
            swampLifeTime -= Time.deltaTime;

            if (swampLifeTime <= 0)
            {
                swampAnimator.SetBool("Live", false);
                swampLifeTime = swampDefaultLifeTime;
            }
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Player"))
        {
            GetComponent<Animator>().SetTrigger("PictureSpit");
            Invoke("Spit", GetComponent<Animator>().runtimeAnimatorController.animationClips[1].length);
        }
    }

    private void Spit()
    {
        mySpit.SetActive(true);
        mySpitAnimator.Play("SpitAnimation");
        Invoke("StopSpit", mySpitAnimator.GetCurrentAnimatorClipInfo(0).Length);
    }

    private void StopSpit()
    {
        mySpit.SetActive(false);

        mySwamp.SetActive(true);
        //swampAnimator.SetBool("Live", true);
        //swampAnimator.Play("DarkPoisonGrowing");
        Invoke("KillSwamp", swampDefaultLifeTime + 1);
    }

    private void KillSwamp()
    {
        mySwamp.SetActive(false);
        mySwamp.GetComponent<DarkSwamp>().alreadyAppliedPoison = false;
    }
}
