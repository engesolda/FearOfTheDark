using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerIntro : MonoBehaviour
{
    //Public
    public float playerSpeed;
    public float cameraSpeed;
    public float closeSpeed;
    public GameObject mirror;
    public Camera mainCamera;

    //Private
    private bool canMove = false;
    private bool startAnimation = false;
    private bool startEyeClose = false;
    private float t = 0f;
    private Animator animator;
    private Animator mirrorAnimator;
    private Camera camera;
    private Color defaultColor;
    private SpriteRenderer myRederer;
    private Vector2 eyeClosePoint = new Vector2(-1.4f, 2.57f);
    private Vector2 nextPoint;
    private Vector2 p1;
    private List<Vector2> points = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        camera = mainCamera.GetComponent<Camera>();
        myRederer = GetComponent<SpriteRenderer>();
        defaultColor = myRederer.color;
        animator = GetComponent<Animator>();
        mirrorAnimator = mirror.GetComponent<Animator>();
        animator.SetBool("PlayerBlinking", true);
        p1 = new Vector2(transform.position.x + 0.8f, transform.position.y);
        points.Add(new Vector2(-0.6f, -0.3f));
        points.Add(new Vector2(-1.6f, -0.3f));
        points.Add(new Vector2(-1.6f, 1.7f));

        InvokeRepeating("LeaveBed1", animator.runtimeAnimatorController.animationClips[0].length, .01f);        
    }

    private void LeaveBed1()
    {
        canMove = true;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            nextPoint = Vector2.MoveTowards(transform.position, p1, Time.fixedDeltaTime * playerSpeed);
            GetComponent<Rigidbody2D>().MovePosition(nextPoint);
            SetAnimation();

            if (Vector2.Distance((Vector2)transform.position, p1) <= 0.0001f)
            {
                startAnimation = true;

                if (points.Count > 0)
                {
                    p1 = points[0];
                    points.RemoveAt(0);
                }
                else
                {
                    animator.SetBool("stopUp", true);
                    startEyeClose = true;
                }
            }
        }

        if (startEyeClose)
        {
            Vector3 nextCameraPos = Vector3.MoveTowards(camera.transform.position, eyeClosePoint, Time.fixedDeltaTime * cameraSpeed);
            nextCameraPos.z = -1;
            camera.transform.position = nextCameraPos;

            if (camera.orthographicSize > 0.01f)
            {
                camera.orthographicSize -= closeSpeed;
            }
            else
            {
                SceneManager.LoadScene("M1L1");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Equals("MirrorAnimationStart"))
        {
            mirrorAnimator.SetTrigger("AnimateMirror");
        }
    }

    //Move and animated the player
    private void SetAnimation()
    {
        if (!startAnimation)
        {
            return;
        }

        Vector2 currPos = transform.position;
        //Vector2 newPos = nextPoint;

        Vector2 dir = GetDirection(currPos, p1);

        animator.SetBool("Up", false);
        //animator.SetBool("Down", false);
        animator.SetBool("Left", false);
        //animator.SetBool("Right", false);

        /*if (dir == Vector2.right)
        {
            animator.SetBool("Boss1Right", true);
        }*/
        if (dir == Vector2.left)
        {
            animator.SetBool("Left", true);
        }
        if (dir == Vector2.up)
        {
            animator.SetBool("Up", true);
        }
        /*if (dir == Vector2.down)
        {
            animator.SetBool("Boss1Down", true);
        }*/
    }

    public void ChangeColor(Color _newColor, float _timeToResume)
    {
        myRederer.color = _newColor;
        Invoke("ResumeColor", _timeToResume);
    }

    private void ResumeColor()
    {
        myRederer.color = defaultColor;
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
