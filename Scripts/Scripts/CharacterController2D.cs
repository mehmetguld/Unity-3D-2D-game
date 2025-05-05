using UnityEngine;
using System.Collections;


public class CharacterController2D : MonoBehaviour
{
    [Header("Movement Settings")] public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    [Header("Combat Settings")] public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float attackCooldown = 0.5f;

    [Header("Audio Settings")] public AudioClip attackSound;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;

    [Header("Visual Effects")] public GameObject hurtEffect;
    public float hurtEffectDuration = 0.5f;

    private Rigidbody2D _rb;
    private Animator _anim;
    private AudioSource _audioSource;

    private float _moveInput;
    private bool _isGrounded;
    private bool _canAttack = true;
    private bool _isAlive = true;
    private bool _isWalking = false;
    private float _lastAttackTime;

    private int _direction = 1;
    private Vector3 _originalScale;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
        
        _originalScale = transform.localScale;
    }

    void Update()
    {
        if (!_isAlive) return;

        // Ground check
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle input
        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    void FixedUpdate()
    {
        if (!_isAlive) return;

        // Apply movement
        _rb.linearVelocity = new Vector2(_moveInput * moveSpeed, _rb.linearVelocity.y);
    }

    void HandleMovement()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");

        if (_moveInput != 0)
        {
            // Flip character
            _direction = _moveInput > 0 ? 1 : -1;
            transform.localScale = new Vector3(_direction * Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);

            // Animation
            if (_isGrounded)
            {
                _anim.SetBool("isRun", true);
                if (!_isWalking)
                {
                    _isWalking = true;
                    if (walkSound != null)
                        _audioSource.PlayOneShot(walkSound, 0.5f);
                }
            }
        }
        else
        {
            _anim.SetBool("isRun", false);
            _isWalking = false;
        }
    }

    void HandleJump()
    {
        if (_isGrounded)
        {
            _anim.SetBool("isJump", false);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
                _anim.SetBool("isJump", true);
                if (jumpSound != null)
                    _audioSource.PlayOneShot(jumpSound);
            }
        }
        else
        {
            _anim.SetBool("isJump", true);
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack && Time.time >= _lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
            _lastAttackTime = Time.time;
        }
    }

    IEnumerator Attack()
    {
        _canAttack = false;
        _anim.SetTrigger("attack");

        if (attackSound != null)
            _audioSource.PlayOneShot(attackSound);

        // Wait for attack animation to reach the right frame
        yield return new WaitForSeconds(0.1f);

        // Spawn projectile
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Start moving the projectile
            StartCoroutine(MoveProjectile(projectile, _direction));
        }

        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true;
    }

    IEnumerator MoveProjectile(GameObject projectile, float moveDirection)
    {
        float lifetime = 3f;
        float elapsedTime = 0f;

        while (projectile != null && elapsedTime < lifetime)
        {
            projectile.transform.position += new Vector3(moveDirection * projectileSpeed * Time.deltaTime, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (projectile != null)
            Destroy(projectile);
    }

    public void TakeDamage()
    {
        if (!_isAlive) return;

        _anim.SetTrigger("hurt");

        if (hurtSound != null)
            _audioSource.PlayOneShot(hurtSound);

        // Show hurt effect
        if (hurtEffect != null)
        {
            GameObject effect = Instantiate(hurtEffect, transform.position, Quaternion.identity);
            Destroy(effect, hurtEffectDuration);
        }

        // Apply knock back
        float knockbackDirection = -_direction;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(new Vector2(knockbackDirection * 5f, 3f), ForceMode2D.Impulse);
    }

    private void Die()
    {
        if (!_isAlive) return;

        _isAlive = false;
        _anim.SetTrigger("die");

        if (dieSound != null)
            _audioSource.PlayOneShot(dieSound);

        // Disable physics
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Restart level after delay
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Example: Take damage when hitting enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }

        // Example: Die when falling into pit
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw ground check radius
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
    
