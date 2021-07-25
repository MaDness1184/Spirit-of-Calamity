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
    transition,
    dead
}

public class Player : MonoBehaviour
{
    [Header("Player State")]
    public PlayerState currentState;
    private bool invulnerable = false;

    [Header("Player Stats")]
    public FloatReference currentHealth;
    private float health;
    public float runSpeed = 5f;

    [Header("Player Damage")]
    public int basicAttackDamage = 2;

    [Header("Coroutines")]
    public float basicDelay = 0.4f;

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
            BasicAttack(); // Attack on button press
        }
    }

    // Change State
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

    // Actions
    private void Idle()
    {
        // Random Idle Animation run through after a random duration
    }

    private void Run()
    {
        changeInVelocity.Normalize(); // 
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

    private void BasicAttack()
    {
        if(Input.GetButtonDown("Basic Attack") && currentState != PlayerState.attack
            && currentState != PlayerState.stagger)
        {
            StartCoroutine(BasicAttackCo());
        }
    }

    public void Hurt(float knocktime, float recoverDelay, float damage) // Start KnockCo and take damage
    {
        if (invulnerable != true) // Take damage if Player is not invulnerable
        {
            TakeDamage(damage);
            if (currentHealth.GetRuntimeValue() > 0f) // Run KnockCo if Player is alive
            {
                StartCoroutine(KnockCo(knocktime, recoverDelay));
            }
        }
    }

    private void TakeDamage(float damage) // Take dmg and update hp
    {
        invulnerable = true;
        if (currentState != PlayerState.stagger && currentHealth.GetRuntimeValue() > 0f) // If Player is alive and not staggered
        {
            currentHealth.SubtractRuntimeValue(damage); // subtract the currentHealth from the scriptable object
            //playerHealthSignal.Raise(); // Raise all listeners of playerHealthSignal
            //cameraShakeSignal.Raise(); // Raise all listeners of playerShake
        }
        if (currentHealth.GetRuntimeValue() <= 0f) // kill off the Player once currentHealth reaches 0
        {
            ChangeState(PlayerState.dead);
            this.gameObject.SetActive(false); // will be put into a coroutine later
            DebugMode(1);
        }
    }

    // Coroutines
    private IEnumerator BasicAttackCo()
    {
        ChangeState(PlayerState.attack);
        myRigidbody2D.velocity = Vector2.zero;
        myAnimator.SetBool("isAttacking", true);
        yield return null;
        myAnimator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(basicDelay);
        ChangeState(PlayerState.idle);
    }

    private IEnumerator KnockCo(float knocktime, float recoverDelay)
    {
        if (myRigidbody2D != null && currentState != PlayerState.stagger)
        {
            ChangeState(PlayerState.stagger);
            yield return new WaitForSeconds(knocktime);
            myRigidbody2D.velocity = Vector2.zero;
            yield return new WaitForSeconds(recoverDelay);
            invulnerable = false;
            ChangeState(PlayerState.idle);
        }
    }

    // Debug Mode
    private void DebugMode(int debugCode)
    {
        if (debugModeEnabler)
        {
            switch (debugCode)
            {
                case 1:
                    Debug.Log("Player Velocity: " + changeInVelocity);
                    break;
                case 2:
                    Debug.Log("");
                    break;

            }
        }
    }
}
