using UnityEngine;

public class EnemyCombat: MonoBehaviour
{
    public Transform enemyAttackPoint;
    public LayerMask playerLayers;

    public float enemyAttackRange = 0.5f;
    public int enemyAttackDamage = 40;


    public void DamagePlayer()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(enemyAttackPoint.position, enemyAttackRange, enemyAttackDamage
);
        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("Zarar" + enemy.name);
            enemy.GetComponent<CharacterHealth>().TakeDamage(enemyAttackDamage);
            FindObjectOfType<AudioManager>().Play("swordsound2");
            FindObjectOfType<AudioManager>().Play("enemyhurt");
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (enemyAttackPoint == null)
            return;


        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
    }
}
