using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - Create a base enemy with all base status and variables that every enemy will extends

public class EnemyWhiteWalker : EnemyBase
{
    //Public
    //public GameObject player;
    public float enemySpeed;
    public float timeBetweenPaths;
    public float distanceToPursuitPlayer;
    public float waitAfterDrag;
    public float life;
    public float timeToFlashRed;
    public float timeDamageLight;
    public int lifeToDrainOnDrag;
    public Color hitColor;

    //Private
    private PathFinderAIv2 pathFinder;
    private List<Vector2> path = new List<Vector2>();
    private Vector2 myPos;
    private Vector2 nextPoint;
    private Vector2 lastTargetPosition;
    private BoxCollider2D myBoxCollider;
    private bool draggingPlayer = false;
    private bool onLight = false;
    private bool stopped = true;
    private float timeToWaitAfterDrag = -1;
    private float timeBetweenDmgLight = 0;
    private SpriteRenderer myRenderer;
    private Color originalColor;    

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        originalColor = myRenderer.color;
        myBoxCollider = GetComponent<BoxCollider2D>();
        lastTargetPosition = transform.position;
        pathFinder = GetComponent<PathFinderAIv2>();

        InvokeRepeating("CallPathFinder", 0, timeBetweenPaths);
    }

    void CallPathFinder()
    {
        if (timeToWaitAfterDrag >= 0)
        {
            timeToWaitAfterDrag -= timeBetweenPaths;
        }
        else
        {
            Player player = Player.GetInstance();
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (player.GetComponent<Player>().IsOnLight())
            {
                if (distanceToPlayer <= distanceToPursuitPlayer && !draggingPlayer)
                {
                    if (Vector2.Distance(transform.position, lastTargetPosition) == 0.0f)
                    {//Find a path to the player

                        path.Clear();
                        path = pathFinder.FindBestPath(transform.position, player.transform.position, myBoxCollider.size.x, myBoxCollider.size.y, distanceToPursuitPlayer);
                    }
                }
            
                if (distanceToPlayer <= 1f && !draggingPlayer)
                {//Find a path to the darkness

                    path.Clear();
                    path = pathFinder.FindBestPathToDarkness(transform.position, myBoxCollider.size.x, myBoxCollider.size.y, distanceToPursuitPlayer);
                    lastTargetPosition = transform.position;
                    draggingPlayer = true;
                }
            }

            if (stopped && onLight)
            {
                path.Clear();
                path = pathFinder.FindBestPathToDarkness(transform.position, myBoxCollider.size.x, myBoxCollider.size.y, 10);
                lastTargetPosition = transform.position;
            }
        }
    }

    void FixedUpdate()
    {
        MoveToPosition(path);

        if (base.dead)
        {
            Destroy(gameObject);
        }

        timeBetweenDmgLight -= Time.deltaTime;

        if (onLight && timeBetweenDmgLight <= 0)
        {
            ApplyDamage(-10);
            timeBetweenDmgLight = timeDamageLight;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Light"))
        {
            onLight = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Light"))
        {
            onLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Light"))
        {
            onLight = false;
        }
    }

    //Move this gameobject using the positions stored into the list "path"
    void MoveToPosition(List<Vector2> _path)
    {
        Player player = Player.GetInstance();

        if (_path.Count > 0)
        {
            stopped = false;

            if (draggingPlayer)
            {// Drag the player with it

                player.GetComponent<MovementController>().DragPlayer(transform.position, _path[0]);
            }

            if (Vector2.Distance(transform.position, lastTargetPosition) == 0.0f)
            {//Get new position to go if the last was reached

                nextPoint = _path[_path.Count - 1];
                _path.RemoveAt(_path.Count - 1);
                lastTargetPosition = nextPoint;
            }
        }
        else
        {
            if (draggingPlayer)
            {//If is dragging the player we can stop

                draggingPlayer = false;
                player.GetComponent<MovementController>().StopDrag(lifeToDrainOnDrag);
                timeToWaitAfterDrag = waitAfterDrag;
            }

            stopped = true;
        }

        if (Vector2.Distance(transform.position, lastTargetPosition) > 0.0f)
        {//Here is the movimentation

            Vector2 newPos = Vector2.MoveTowards(transform.position, nextPoint, Time.deltaTime * enemySpeed);
            GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
    }

    public override void ApplyDamage(float _amount)
    {
        life += _amount;
        myRenderer.color = hitColor;
        Invoke("NormalizeColor", timeToFlashRed);

        if (life <= 0)
        {
            base.dead = true;
        }
    }

    private void NormalizeColor()
    {
        myRenderer.color = originalColor;
    }
}
