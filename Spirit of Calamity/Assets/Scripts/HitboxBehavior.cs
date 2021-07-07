using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxBehavior : MonoBehaviour
{
    [Header("Properties")]
    public string hitboxName;
    public bool canBreakObjects = false;

    [Header("Knockback")]
    public bool hasKnockback;
    public float thrust = 2f; // The amount of thrust applied to the object collided w/
    public float knocktime = 0.2f; // The amount of time the object is in stagger
    public float recoverDelay = 0f;

    [Header("Damage")]
    public float damage = 0;

    [Header("Debug")]
    public bool debugModeEnabler = false;

    void Start()
    {
        if (!hasKnockback) // No Knockback if hasKnockback = false
        {
            KnockbackDisabler();
        }
    }

    // On Collision
    private void OnTriggerEnter2D(Collider2D targetCollider) // When the target's collider is hit by the attacker's collider
    {
        if (targetCollider.gameObject.CompareTag("Enemy") || targetCollider.gameObject.CompareTag("Player")
                && targetCollider.isTrigger) // If Enemy or Player
        {
            KnockbackAndDamage(targetCollider);
        }
        else if (canBreakObjects == true) // Can Hitbox break objects?
        {
            if (targetCollider.gameObject.CompareTag("Breakable")) // Is the object of type "Breakable"
            {
                //BreakObject(targetCollider);
            }
        }
    }

    // Breakable Objects
    private void BreakObject(Collider2D targetCollider) // When the attackers collider comes in contact with object on layer "Breakable"
    {
         //StartCoroutine(targetCollider.GetComponent<Breakable>().BreakCo());
    }

    // Knockback
    private void KnockbackDisabler() // Set all knockback variables to 0
    {
         thrust = 0f;
         knocktime = 0f;
         recoverDelay = 0f;
    }

    private void KnockbackAndDamage(Collider2D targetCollider) // When the attacker's collider comes in contact with object of type "Enemy" / "Player"
    {
            Rigidbody2D targetRigidbody2D = targetCollider.GetComponent<Rigidbody2D>();

            Vector2 targetOffset = targetRigidbody2D.transform.position - transform.position; // help determine direction by finding the difference
                                                                                              // of the targets transform - who ever has this scrip's transform
            if (targetRigidbody2D != null && targetRigidbody2D.gameObject.CompareTag("Enemy")
                   && targetRigidbody2D.GetComponent<Enemy>().currentState != EnemyState.stagger
                   && targetRigidbody2D.GetComponent<Enemy>().currentState != EnemyState.dead) // Enemy collision
            {
                DebugMode(1);
                targetRigidbody2D.GetComponent<Enemy>().Hurt(targetRigidbody2D, knocktime, recoverDelay, damage, KnockDirection(targetOffset)); // Start KnockCo and take dmg
            }

            if (targetRigidbody2D != null && targetRigidbody2D.gameObject.CompareTag("Player")
                    && targetRigidbody2D.GetComponent<Player>().currentState != PlayerState.stagger
                    && targetRigidbody2D.GetComponent<Player>().currentState != PlayerState.dead) // Player collision
            {
                DebugMode(2);
                targetRigidbody2D.AddForce(KnockDirection(targetOffset), ForceMode2D.Impulse); // Force and direction applied to collision
                targetRigidbody2D.GetComponent<Player>().Hurt(knocktime, recoverDelay, damage); // Start KnockCo and take dmg
            }
    }

    private Vector2 KnockDirection(Vector2 targetOffset) // Direction of knockback
    {
        if (Mathf.Abs(targetOffset.x) > Mathf.Abs(targetOffset.y)) // if otherCollision positioned right/left 
        {
            if (targetOffset.x > 0) // if right
            {
                targetOffset = new Vector2(1f, 0f);
            }
            else // if left
            {
                targetOffset = new Vector2(-1f, 0f);
            }
        }
        if (Mathf.Abs(targetOffset.x) < Mathf.Abs(targetOffset.y)) // if otherCollision positioned up/down 
        {
            if (targetOffset.y > 0) // if Up
            {
                targetOffset = new Vector2(0f, 1f);
            }
            else // if down
            {
                targetOffset = new Vector2(0f, -1f);
            }
        }
        targetOffset = targetOffset * thrust; // Implement thrust amt.
        return targetOffset;
    }

    private void DebugMode(int debugCode)
    {
        if (debugModeEnabler)
        {
            switch (debugCode)
            {
                case 1:
                    Debug.Log("Player did " + damage + " damage with " + hitboxName + "!");
                    break;
                case 2:
                    Debug.Log(this.GetComponent<Enemy>().enemyName + " did " + damage + " damage with " + hitboxName + "!");
                    break;
            }
        }
    }

}
