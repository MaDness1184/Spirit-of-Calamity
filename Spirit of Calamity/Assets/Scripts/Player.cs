using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    idle,
    walk,
    attack,
    interact,
    stagger,
    transition
}

public class Player : MonoBehaviour
{
    [Header("Player State")]
    public PlayerState currentState;
    //private bool invulnerable = false;

    [Header("Player Stats")]
    public float health;
    public float runSpeed = 5f;

    [Header("Debug")]
    public bool debugModeEnabler = false;

    //Private Chached References
    private Vector2 changeInVelocity;
    private Rigidbody2D myRigidbody2D;
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != PlayerState.attack && currentState != PlayerState.stagger
            && currentState != PlayerState.transition && currentState != PlayerState.interact) // If Player is not attacking / staggered / transitioning
        {
            Run(); // Move the character every frame
            //Attack(); // Attack on button press
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    private void ChangeState(PlayerState newState, PlayerState oldState)
    {
        if (currentState == oldState)
        {
            currentState = newState;
        }
    }

    private void Run()
    {
        changeInVelocity.Normalize();
        changeInVelocity.x = Input.GetAxisRaw("Horizontal") * runSpeed; // (1, 0 or -1) * walkspeed * seconds from the last frame
        changeInVelocity.y = Input.GetAxisRaw("Vertical") * runSpeed; // (1, 0 or -1) * walkspeed * seconds from the last frame
        myRigidbody2D.velocity = changeInVelocity;
        DebugMode(1);

        bool playerIsMoving = Mathf.Abs(myRigidbody2D.velocity.x) > Mathf.Epsilon
           || Mathf.Abs(myRigidbody2D.velocity.y) > Mathf.Epsilon;
        if (playerIsMoving) // If velocity magnitude > Epsilon
        {
            if (Mathf.Abs(changeInVelocity.x) > Mathf.Abs(changeInVelocity.y))
            {
                myAnimator.SetFloat("moveX", changeInVelocity.x); // change idle animation relative to where the Player is facing
                myAnimator.SetFloat("moveY", 0);
            }
            else
            {
                myAnimator.SetFloat("moveX", 0);
                myAnimator.SetFloat("moveY", changeInVelocity.y); // change idle animation relative to where the Player is facing
            }
            myAnimator.SetBool("isRunning", true);
            ChangeState(PlayerState.walk);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);
            ChangeState(PlayerState.idle); // change to idle if player is not walking
        }
    }

    private void DebugMode(int debugCode)
    {
        if (debugModeEnabler)
        {
            switch (debugCode)
            {
                case 1:
                    Debug.Log("Player Velocity: " + changeInVelocity);
                    break;
            }
        }
    }
}
