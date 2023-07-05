using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //Public

    //Private
    private bool playerStopped = true;
    private Animator animator;
    private StatsController myStats;
    private SoundController mySounds;
    private Player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        myStats = GetComponent<StatsController>();
        playerScript = GetComponent<Player>();
        mySounds = GetComponent<SoundController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
    }

    //Move and animated the player
    private void MovePlayer()
    {
        if (!myStats.beignDragged)
        {
            Vector2 newPos = transform.position;
            animator.SetBool("Up", false);
            animator.SetBool("Down", false);
            animator.SetBool("Left", false);
            animator.SetBool("Right", false);
            playerScript.flashLight.transform.rotation = Quaternion.Euler(90, 90, 0);

            if (Input.GetKey("up"))
            {
                animator.SetBool("Up", true);
                newPos.y += 1;
                playerScript.flashLight.transform.rotation = Quaternion.Euler(270, 90, 0);
            }
            if (Input.GetKey("down"))
            {
                animator.SetBool("Down", true);
                newPos.y += -1;
            }
            if (Input.GetKey("left"))
            {
                animator.SetBool("Left", true);
                newPos.x += -1;
                playerScript.flashLight.transform.rotation = Quaternion.Euler(180, 90, 0);
            }
            if (Input.GetKey("right"))
            {
                animator.SetBool("Right", true);
                newPos.x += 1;
                playerScript.flashLight.transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            playerStopped = (newPos == (Vector2)transform.position) ? true : false;

            mySounds.PlayWalkOnWood(playerStopped);

            newPos = Vector2.MoveTowards(transform.position, newPos, Time.fixedDeltaTime * myStats.playerSpeed);
            GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
    }

    //Put the player on draggin state and move it to the requested position
    public void DragPlayer(Vector2 _enemyPos, Vector2 _endPos)
    {
        myStats.beignDragged = true;

        if (Vector2.Distance(transform.position, _enemyPos) >= 0.5f)
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position, _enemyPos, Time.fixedDeltaTime * myStats.playerSpeed);
            GetComponent<Rigidbody2D>().MovePosition(newPos);
        }
    }

    public void StopDrag(int _lifeToLoseOnDrag)
    {
        playerScript.ApplyDamageToPlayer(_lifeToLoseOnDrag);
        myStats.beignDragged = false;
    }
}
