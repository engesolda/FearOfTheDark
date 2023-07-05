using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : EnemyBase
{
    //Public
    //public GameObject player;
    public int numTryNewPath;
    public float life;
    public float speed;
    public float distanceToPlayer;
    public float timeBetweenPaths;
    public float timeBetweenBH;
    public float chanceToBH;
    public float blackHoleSpeed;
    public float chanceToJump;
    public float jumpHeight;
    public float jumpSpeed;
    public float timeToFall;
    public float maxShadowSize;
    public float shadowAnimationSpeed;
    public float timeToFlashRed;
    public float drainingTime;
    public float drainOverTime;
    public float chanceRandomMove;
    public float timeBetweenRandomMove;
    public float distanceToMove;
    public float maxTimeWithoutAttack;
    public int drainAmout;

    //Private
    private bool jumping = false;
    private bool goingUp = false;
    private bool atTop = false;
    private bool falling = false;
    private bool landed = false;
    private bool vunerable = true;
    private bool mustAttack = false;
    private float defaultDrainingTime;
    private float targetJumpHeight = 0;
    private float defaultTimeToFall;
    private float shadowGrowthSpeed;
    private float maxLife;
    private float lastDistancePath = 0f;
    private float timeWithoutAttack;
    private int defaultNumberTriesPaths;
    private Vector2 endJumpPosition;
    private Vector2 lastTargetPosition;
    private Vector2 nextPoint = Vector2.zero;
    private PathFinderAIv2 pathFinder;
    private List<Vector2> path = new List<Vector2>();
    private BoxCollider2D myBoxCollider;
    private Transform playerTransform;
    private GameObject blackHoles;
    private GameObject shadowResource;
    private GameObject myShadow;
    private SpriteRenderer myRenderer;
    private Animator myAnimator;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        //Initializations
        myRenderer = GetComponent<SpriteRenderer>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        pathFinder = GetComponent<PathFinderAIv2>();
        var emission = GetComponent<ParticleSystem>().emission;
        myAnimator = GetComponent<Animator>();

        blackHoles = Resources.Load("BlackHole") as GameObject;
        shadowResource = Resources.Load("Boss1Shadow") as GameObject;
        myShadow = Instantiate(shadowResource) as GameObject;

        originalColor = myRenderer.color;
        defaultTimeToFall = timeToFall;
        playerTransform = Player.GetInstance().transform;
        lastTargetPosition = transform.position;
        emission.enabled = false;
        defaultDrainingTime = drainingTime;
        defaultNumberTriesPaths = numTryNewPath;
        timeWithoutAttack = maxTimeWithoutAttack;
        maxLife = life;

        
        InvokeRepeating("PerformAttack", timeBetweenBH, timeBetweenBH);
        InvokeRepeating("Move", timeBetweenPaths, timeBetweenPaths);
        InvokeRepeating("MoveRandom", timeBetweenRandomMove, timeBetweenRandomMove);
    }

    private void FixedUpdate()
    {
        timeWithoutAttack += Time.deltaTime;
        if (timeWithoutAttack >= maxTimeWithoutAttack)
        {
            mustAttack = true;
            timeWithoutAttack = 0f;
        }


        if (jumping)
        {
            JumpOntoPlayer();
        }
        else
        {
            MoveToPosition();
        }

        if (myShadow && myShadow.activeSelf)
        {
            CastShadow();
        }

        if (base.dead)
        {
            Destroy(gameObject);
        }
    }

    private void PerformAttack()
    {
        if (Vector2.Distance(playerTransform.position, transform.position) < distanceToPlayer * 2f || mustAttack)
        {//If player is too close we try to attack with balck holes

            if (Random.Range(0, 100) <= chanceToBH || mustAttack)
            {
                GameObject blackHole = Instantiate(blackHoles) as GameObject;
                blackHole.transform.position = transform.position;
                blackHole.GetComponent<BlackHole>().SetDestinationPoint(playerTransform.position);
                mustAttack = false;
            }
        }

        if ((Random.Range(0, 100) <= chanceToJump && !jumping) || mustAttack)
        {//Jump onto player

            jumping = true;
            goingUp = true;
            GetComponent<BoxCollider2D>().enabled = false; //Disable the collider during jump to not stop on walls and stuff
            targetJumpHeight = playerTransform.position.y >= transform.position.y ? playerTransform.position.y + jumpHeight : transform.position.y + jumpHeight;

            myShadow.transform.position = transform.position;
            myShadow.transform.localScale = new Vector3(1, 1, 1);
            myShadow.SetActive(true);
            shadowGrowthSpeed = targetJumpHeight / shadowAnimationSpeed;
            mustAttack = false;
        }
    }

    private void JumpOntoPlayer()
    {
        Vector2 newPos = transform.position;

        if (goingUp)
        {//Jumping

            if (newPos.y > targetJumpHeight)
            {
                goingUp = false;
                atTop = true;
                endJumpPosition = playerTransform.position;
            }

            newPos = Vector2.MoveTowards(transform.position, transform.position + new Vector3(0, 1, 0), Time.deltaTime * jumpSpeed);
        }

        if (atTop)
        {//Reached the top

            transform.position = new Vector3(endJumpPosition.x, playerTransform.position.y + jumpHeight, 0);

            myShadow.transform.position = endJumpPosition;

            defaultTimeToFall -= Time.deltaTime;

            if (defaultTimeToFall <= 0)
            {
                atTop = false;
                falling = true;
                defaultTimeToFall = timeToFall;
            }
        }

        if (falling)
        {//Falling

            if (newPos.y == endJumpPosition.y)
            {
                falling = false;
                landed = true;
            }

            newPos = Vector2.MoveTowards(transform.position, endJumpPosition, Time.deltaTime * jumpSpeed);
        }

        if (landed)
        {//Landed

            landed = false;
            GetComponent<BoxCollider2D>().enabled = true;
            lastTargetPosition = transform.position;
            nextPoint = transform.position;

            var emission = GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
            InvokeRepeating("CheckDrainingArea", 0, drainOverTime);
        }

        GetComponent<Rigidbody2D>().MovePosition(newPos);
    }

    private void CastShadow()
    {
        if (myShadow.transform.localScale.x < maxShadowSize && !falling)
        {
            myShadow.transform.localScale = new Vector3(myShadow.transform.localScale.x + (1 * shadowGrowthSpeed), myShadow.transform.localScale.y + (shadowGrowthSpeed), 1);
        }

        if (myShadow.transform.localScale.x > 0 && falling)
        {
            myShadow.transform.localScale = new Vector3(myShadow.transform.localScale.x - (1 * shadowGrowthSpeed), myShadow.transform.localScale.y - (shadowGrowthSpeed), 1);
        }

        if (Vector2.Distance(transform.position, endJumpPosition) <= 0.000001f)
        {
            myShadow.SetActive(false);
            myShadow.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void Move()
    {
        if (Vector2.Distance((Vector2)playerTransform.position, transform.position) <= distanceToPlayer)
        {//Keep a safety distance to the player

            if (Vector2.Distance(transform.position, lastTargetPosition) <= 0.000001f)
            {
                path.Clear();
                path = pathFinder.FindPathToKeepDistance(playerTransform.position, transform.position, distanceToPlayer, transform.localScale.x, transform.localScale.y, distanceToPlayer);
            }
        }
    }

    private void MoveRandom()
    {
        if (Vector2.Distance((Vector2)playerTransform.position, transform.position) > distanceToPlayer)
        {
            if (Random.Range(0, 100) <= chanceRandomMove)
            {
                path.Clear();
                path = pathFinder.RandomMove(transform.position, playerTransform.position, distanceToMove, distanceToPlayer, transform.localScale.x, transform.localScale.y, distanceToPlayer);
            }
        }
    }

    //Move this gameobject using the positions stored into the list "path"
    void MoveToPosition()
    {
        Vector2 playerDirection = GetDirection(transform.position, playerTransform.position);
        SetBossAnimation(playerDirection);
        float distToNextPos = Vector2.Distance(transform.position, lastTargetPosition);

        if (path.Count > 0)
        {
            if (distToNextPos <= 0.000001f)
            {//Get new position to go if the last was reached

                nextPoint = path[path.Count - 1];
                path.RemoveAt(path.Count - 1);
                lastTargetPosition = nextPoint;
                numTryNewPath = defaultNumberTriesPaths;
            }
        }

        if (distToNextPos > 0.0f)
        {//Here is the movimentation

            if (Vector2.Distance(nextPoint, lastTargetPosition) <= lastDistancePath)
            {//Increment the times we tried to reach this position

                numTryNewPath--;
            }

            if (numTryNewPath <= 0)
            {//If we reached the number of tries to reach the place its because our object is stuck

                path.Clear();
                path = pathFinder.FindPathToKeepDistance(playerTransform.position, transform.position, distanceToPlayer, transform.localScale.x, transform.localScale.y, distanceToPlayer);
                numTryNewPath = defaultNumberTriesPaths;
            }

            Vector2 newPos = Vector2.MoveTowards(transform.position, nextPoint, Time.deltaTime * speed);
            GetComponent<Rigidbody2D>().MovePosition(newPos);
            Vector2 direction = GetDirection(transform.position, nextPoint);
            SetBossAnimation(direction);
            lastDistancePath = Vector2.Distance(newPos, lastTargetPosition);
        }
    }

    private void SetBossAnimation (Vector2 _direction)
    {
        myAnimator.SetBool("Boss1Right", false);
        myAnimator.SetBool("Boss1Left", false);
        myAnimator.SetBool("Boss1Up", false);
        myAnimator.SetBool("Boss1Down", false);

        if (_direction == Vector2.right)
        {
            myAnimator.SetBool("Boss1Right", true);
        }
        if (_direction == Vector2.left)
        {
            myAnimator.SetBool("Boss1Left", true);
        }
        if (_direction == Vector2.up)
        {
            myAnimator.SetBool("Boss1Up", true);
        }
        if (_direction == Vector2.down)
        {
            myAnimator.SetBool("Boss1Down", true);
        }
    }

    public override void ApplyDamage(float _amount)
    {
        if (vunerable)
        {
            life += _amount;

            if (_amount > 0)
            {
                myRenderer.color = Color.green;
            }
            else
            {
                myRenderer.color = Color.red;
            }

            Invoke("NormalizeColor", timeToFlashRed);

            if (life <= 0)
            {
                base.dead = true;
            }
        }
        else
        {//He still can receive life when is immortal

            if (_amount > 0)
            {
                life += _amount;

                if (life >= maxLife)
                {
                    life = maxLife;
                }

                myRenderer.color = Color.green;

                Invoke("NormalizeColor", timeToFlashRed);
            }
        }
    }

    private void NormalizeColor()
    {
        myRenderer.color = originalColor;
    }

    private void CheckDrainingArea()
    {
        float teste = GetComponent<ParticleSystem>().shape.radius;
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, GetComponent<ParticleSystem>().shape.radius * 3);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        defaultDrainingTime -= drainOverTime;
        vunerable = false;

        if (defaultDrainingTime <= 0)
        {
            defaultDrainingTime = drainingTime;
            CancelInvoke("CheckDrainingArea");
            var emission = GetComponent<ParticleSystem>().emission;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            jumping = false;
            vunerable = true;
            emission.enabled = false;
        }

        foreach (Collider2D collision in collisions)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<Player>().ApplyDamageToPlayer(drainAmout);
                ApplyDamage(drainAmout*-1);                
            }
        }
    }

    //Return a normalized vector with the direction
    Vector2 GetDirection(Vector2 _iniPos, Vector2 _endPos)
    {
        Vector2 direction = _endPos - _iniPos;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            if (direction.x >= 0)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
        else
        {
            if (direction.y >= 0)
            {
                return Vector2.up;
            }
            else
            {
                return Vector2.down;
            }
        }
    }
}
