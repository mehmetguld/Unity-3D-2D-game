using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;


    public void DamageEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("Zarar" + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            FindObjectOfType<AudioManager>().Play("swordsound2");
            FindObjectOfType<AudioManager>().Play("enemyhurt");
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;


        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
