using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEye : EnemyBase
{
    //Public 
    public float minTimeBetweenStates;
    public float maxTimeBetweenStates;
    public float distanceToFocus;
    public float timeKeepEyeClosed;
    public float playerSpeedToReduce;
    public float timePlayerSpeedReduced;
    public float timeBetweenCheckPlayer;
    public float distanceToDeactivate;
    //public GameObject player;
    public Color slowColor;

    //Private
    private float timeChangeAnimation;
    private float timeUtilOpenEye;
    private float timeToCheckPlayer;
    private float distanceToStare;
    private float distanceToPlayer;
    private string[] animations;
    private bool focusOnPlayer = false;
    private bool wasHit = false;
    private Animator myAnimator;
    //private StatsController playerStats;    
    private Color defaultColor;
    private Color currentColor;

    // Start is called before the first frame update
    void Start()
    {
        //playerStats = player.GetComponent<StatsController>();
        defaultColor = GetComponent<SpriteRenderer>().color;
        myAnimator = GetComponent<Animator>();

        slowColor.a = 1;
        currentColor = defaultColor;
        distanceToStare = distanceToFocus * 1.8f;
        timeToCheckPlayer = timeBetweenCheckPlayer;
        timeChangeAnimation = maxTimeBetweenStates;
        animations = new string[] { "LookLeft", "LookRight", "EnemyBlink", "EyeUp", "EyeDown" };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (OnLight())
        {
            GetComponent<SpriteRenderer>().color = Color.clear;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = defaultColor;

            distanceToPlayer = Vector2.Distance(Player.GetInstance().transform.position, transform.position);

            if (!wasHit)
            {
                SetEyeAnimation();

                CheckFocusOnPlayer();
            }
        }

        GetComponent<SpriteRenderer>().color = currentColor;
    }

    private bool OnLight()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, distanceToDeactivate, 1 << LayerMask.NameToLayer("Default"));


        foreach (Collider2D collision in collisions)
        {
            if (collision.name.Equals("Light"))
            {
                return true;
            }
        }

        return false;
    }

    private void SetEyeAnimation()
    {
        if (distanceToPlayer <= distanceToStare)
        {
            Vector2 playerDirection = Player.GetInstance().transform.position - transform.position;
            playerDirection = playerDirection.normalized;
            playerDirection = new Vector2(Mathf.RoundToInt(playerDirection.x), Mathf.RoundToInt(playerDirection.y));

            if (playerDirection == Vector2.up)
            {
                myAnimator.SetTrigger("EyeUp");
            }
            else if (playerDirection == Vector2.down)
            {
                myAnimator.SetTrigger("EyeDown");
            }
            else if (playerDirection == Vector2.left)
            {
                myAnimator.SetTrigger("LookLeft");
            }
            else if (playerDirection == Vector2.right)
            {
                myAnimator.SetTrigger("LookRight");
            }
        }
        else
        {
            timeChangeAnimation -= Time.deltaTime;

            if (timeChangeAnimation <= 0)
            {
                myAnimator.SetTrigger(animations[Random.Range(0, animations.Length)]);

                timeChangeAnimation = Random.Range(minTimeBetweenStates, maxTimeBetweenStates);
            }
        }
    }

    private void CheckFocusOnPlayer()
    {
        if (distanceToPlayer <= distanceToFocus && !focusOnPlayer)
        {
            currentColor = slowColor;
            Player.GetInstance().ChangeSpeed(playerSpeedToReduce, timePlayerSpeedReduced);
            focusOnPlayer = true;
            Invoke("ResumeEyeColor", timePlayerSpeedReduced);
            Invoke("ChangePlayerColor", 0);
        }
        else
        {
            timeToCheckPlayer -= Time.deltaTime;

            if (distanceToPlayer > distanceToFocus || timeToCheckPlayer <= 0)
            {
                timeToCheckPlayer = timeBetweenCheckPlayer;
                focusOnPlayer = false;
            }
        }
    }

    private void ChangePlayerColor()
    {
        Player.GetInstance().ChangeColor(slowColor, timePlayerSpeedReduced);
    }

    private void ResumeEyeColor()
    {
        currentColor = defaultColor;
    }

    public override void ApplyDamage(float _amount)
    {
        myAnimator.SetBool("EyeClosed", true);
        Invoke("OpenEye", timeKeepEyeClosed);
        wasHit = true;
    }

    private void OpenEye()
    {
        myAnimator.SetBool("EyeClosed", false);
        wasHit = false;
    }
}
