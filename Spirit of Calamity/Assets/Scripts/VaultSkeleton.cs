using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultSkeleton : Enemy
{
    [Header("Movement")]
    public Transform target;
    public float chaseRadius = 5f;
    public float attackRadius = 0.5f;
    public Vector2 homePosition;

    [Header("Coroutines")]

    // public cache
    [HideInInspector] public Rigidbody2D myRidgidBody; // made public for PatrolLog
    [HideInInspector] public Animator myAnimator; // made public for PatrolLog

    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.idle;
        myRidgidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform; // plug in the player's position in the world
        homePosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDistance();
    }

    // Actions
    public virtual void CheckDistance() // Wake up / move / fall asleep; made virtual so that PatrolLog can override with it's own CheckDistance
    {
        if (Vector2.Distance(target.position, transform.position) <= chaseRadius // Check if the target's distance is close enough
                && Vector2.Distance(target.position, transform.position) > attackRadius)
        {
            MoveToTarget(); // Move towards the target
        }
        /*
        else if (Vector2.Distance(target.position, transform.position) > chaseRadius)// Target out of range = fall back asleep
        {
            if (currentState != EnemyState.stagger)
            {
                MoveToHomePosition();
            }
        }
        */
        else
        {
            ChangeState(EnemyState.idle);
            myAnimator.SetBool("isWalking", false);
        }
    }

    private void MoveToTarget() // Move towards the target if not staggered or sleeping
    {
        if (currentState != EnemyState.stagger)
        {
            ChangeState(EnemyState.walk);
            myAnimator.SetBool("isWalking", true);
            Vector2 temp = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            ChangeAnimDirection(temp - Vector3Extension.AsVector2(transform.position));
            myRidgidBody.MovePosition(temp);
        }
    }

    public void MoveToHomePosition() // The log will to back to it's home position and sleep when the player is out of range
    {
        ChangeState(EnemyState.walk);
        Vector2 temp = Vector2.MoveTowards(transform.position, homePosition, moveSpeed * Time.deltaTime);
        ChangeAnimDirection(temp - Vector3Extension.AsVector2(transform.position));
        myRidgidBody.MovePosition(temp);
    }

    public void ChangeAnimDirection(Vector2 direction) // Change the direction of moveX and moveY
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0) // moving right
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0) // moving left
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0) // moving up
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0) // moving down
            {
                SetAnimFloat(Vector2.down);
            }
        }
    }

    private void SetAnimFloat(Vector2 setVector) // set float for moveX and moveY
    {
        myAnimator.SetFloat("moveX", setVector.x);
        myAnimator.SetFloat("moveY", setVector.y);
    }

    public void isHurt() // hurt animation if staggered from Enemy script
    {
        //myAnimator.SetTrigger("hurtTrigger");
    }

    // Coroutines

}
