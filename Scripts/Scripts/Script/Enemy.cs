using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public Animator anim;
    public int maxHealth = 100;
    int currentHealth;

    EnemyAI enemyAi;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAi = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        void Die()
        {

            anim.SetBool("IsDead", true);

            this.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            enemyAi.followspeed = 0;
            Destroy(gameObject, 2f);


        }
    }
}
