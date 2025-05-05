using UnityEngine;
using System.Collections;


public class EnemyController2D : MonoBehaviour
{
    public enum EnemyType
    {
        Patrol,
        Chaser
    }

    [Header("Enemy Type")] public EnemyType enemyType = EnemyType.Patrol;

    [Header("Movement Settings")] public float moveSpeed = 3f;
    public float patrolWaitTime = 1f;

    [Header("Patrol Settings")] public Transform patrolPoint1;
    public Transform patrolPoint2;
    private bool _movingToPoint2 = true;
    private bool _isWaiting = false;

    [Header("Chaser Settings")] public float detectionRange = 5f;
    public float attackRange = 1f;
    private GameObject _player;
    private bool _isChasing = false;

    [Header("Health Settings")] public int maxHealth = 3;
    private int _currentHealth;

    [Header("Audio Settings")] public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;
    public AudioClip walkSound;
    private bool _isPlayingWalkSound = false;

    [Header("Visual Effects")] public GameObject hurtEffect;
    public float hurtEffectDuration = 0.5f;

    private Rigidbody2D _rb;
    private Animator _anim;
    private AudioSource _audioSource;
    private bool _isAlive = true;
    private int _direction = 1;
    private Vector3 _originalScale;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _currentHealth = maxHealth;


        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();


        _originalScale = transform.localScale;


        _player = GameObject.FindGameObjectWithTag("Player");


        if (enemyType == EnemyType.Patrol && patrolPoint1 != null)
        {
            transform.position = patrolPoint1.position;
        }
    }

    void Update()
    {
        if (!_isAlive) return;

        switch (enemyType)
        {
            case EnemyType.Patrol:
                PatrolBehavior();
                break;
            case EnemyType.Chaser:
                ChaserBehavior();
                break;
        }
    }

    void PatrolBehavior()
    {
        if (patrolPoint1 == null || patrolPoint2 == null || _isWaiting) return;


        Transform targetPoint = _movingToPoint2 ? patrolPoint2 : patrolPoint1;


        float direction = Mathf.Sign(targetPoint.position.x - transform.position.x);
        SetDirection((int)direction);

        _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);
        _anim.SetBool("isRun", true);


        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    void ChaserBehavior()
    {
        if (_player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);


        if (distanceToPlayer <= detectionRange)
        {
            _isChasing = true;


            float direction = Mathf.Sign(_player.transform.position.x - transform.position.x);
            SetDirection((int)direction);

            if (distanceToPlayer > attackRange)
            {
                _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);
                _anim.SetBool("isRun", true);
            }
            else
            {
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
                _anim.SetBool("isRun", false);

            }
        }
        else
        {
            _isChasing = false;
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            _anim.SetBool("isRun", false);
        }
    }

    IEnumerator WaitAndTurn()
    {
        _isWaiting = true;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        _anim.SetBool("isRun", false);

        yield return new WaitForSeconds(patrolWaitTime);

        _movingToPoint2 = !_movingToPoint2;
        _isWaiting = false;
    }

    void SetDirection(int dir)
    {
        if (_direction != dir)
        {
            _direction = dir;

            transform.localScale =
                new Vector3(_direction * Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
        }
    }

    private void TakeDamage()
    {
        if (!_isAlive) return;

        _currentHealth--;
        _anim.SetTrigger("hurt");


        if (hurtSound != null)
            _audioSource.PlayOneShot(hurtSound);


        if (hurtEffect != null)
        {
            GameObject effect = Instantiate(hurtEffect, transform.position, Quaternion.identity);
            Destroy(effect, hurtEffectDuration);
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        _isAlive = false;
        _anim.SetTrigger("die");
        _rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        _rb.bodyType = RigidbodyType2D.Kinematic;


        Destroy(gameObject, 2f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isAlive) return;


        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterController2D playerController = collision.gameObject.GetComponent<CharacterController2D>();
            if (playerController != null)
            {
                playerController.TakeDamage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isAlive) return;


        if (collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage();
            Destroy(collision.gameObject);
        }
    }


    void OnDrawGizmos()
    {
        if (enemyType == EnemyType.Patrol)
        {

            if (patrolPoint1 != null && patrolPoint2 != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(patrolPoint1.position, patrolPoint2.position);
                Gizmos.DrawWireSphere(patrolPoint1.position, 0.3f);
                Gizmos.DrawWireSphere(patrolPoint2.position, 0.3f);
            }
        }
        else if (enemyType == EnemyType.Chaser)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);


            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

    void OnDrawGizmosSelected()
    {

        if (enemyType == EnemyType.Chaser)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, detectionRange);

            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, attackRange);
        }
    }
}
    
