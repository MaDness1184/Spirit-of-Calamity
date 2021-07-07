using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger,
    dead,
    dummy
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy State")]
    public EnemyState currentState;
    public bool dummyModeEnabler = false;
    private bool invulnerable = false;


    [Header("Enemy Stats")]
    public FloatReference maxHealth;
    private float health;
    public float moveSpeed = 2f;

    [Header("Enemy Properties")]
    public string enemyName;
    //public GameObject deathEffect;

    [Header("Debug")]
    public bool debugModeEnabler = false;

    // Awake
    private void Awake()
    {
        health = maxHealth.GetValue();
    }
    
    // Change State
    public void ChangeState(EnemyState newState) // Change EnemyState
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    private void ChangeState(EnemyState newState, EnemyState oldState)
    {
        if (currentState == oldState)
        {
            currentState = newState;
        }
    }

    // Actions
    public void Hurt(Rigidbody2D myRigidbody2D, float knocktime, float recoverDelay, float damage, Vector2 knockDirection) // Start KnockCo and take damage
    {
        if (invulnerable != true)
        {
            if (currentState != EnemyState.stagger && health > 0f) // Take Damage
            {
                invulnerable = true; // Invulnerable
                health -= damage;
                StartCoroutine(KnockCo(myRigidbody2D, knocktime, recoverDelay, knockDirection));
            }
            if (health <= 0f) // Die
            {
                invulnerable = true; // Invulnerable
                Death();
            }
        }
    }

    private void Death()
    {
        ChangeState(EnemyState.dead);
        this.gameObject.SetActive(false); // will be put into a coroutine later
        DebugMode(1);
    }

    // Coroutines
    private IEnumerator KnockCo(Rigidbody2D myRigidbody2D, float knocktime, float recoverDelay, Vector2 knockDirection)
    {
        if (myRigidbody2D != null && currentState != EnemyState.stagger)
        {
            ChangeState(EnemyState.stagger);
            myRigidbody2D.AddForce(knockDirection, ForceMode2D.Impulse); // Force and direction applied to collision
            myRigidbody2D.GetComponent<VaultSkeleton>().isHurt();
            yield return new WaitForSeconds(knocktime);
            myRigidbody2D.velocity = Vector2.zero;
            yield return new WaitForSeconds(recoverDelay);
            invulnerable = false;
            currentState = EnemyState.idle;
        }
    }

    private void DebugMode(int debugCode)
    {
        if (debugModeEnabler)
        {
            switch (debugCode)
            {
                case 1:
                    Debug.Log(enemyName + " has died.");
                    break;
            }
        }
    }
}
