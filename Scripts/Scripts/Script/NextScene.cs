using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public string Level2; //sahneadýeklenecek

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            
            if (EnemyDefeated())
            {
                SceneManager.LoadScene(Level2); //sahneadýeklenecek 
            }
        }
    }

    bool EnemyDefeated()
    {
        
        return GameObject.FindGameObjectWithTag("Enemy") == null;
    }
}