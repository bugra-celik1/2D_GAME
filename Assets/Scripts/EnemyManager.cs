using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyManager : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;

    public float moveSpeed = 3f;
    public float followRange = 10f;       // D��man�n oyuncuyu takip edece�i mesafe
    public float attackRange = 1.5f;      // D��man�n sald�raca�� mesafe
    public float attackCooldown = 1.5f;   // Sald�r�lar aras� bekleme s�resi

    public Transform player;
    public Slider slider;

    private Rigidbody2D rb;
    private bool canAttack = true;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        slider.maxValue = health;
        slider.value = health;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange && distanceToPlayer > attackRange)
        {
            // Oyuncuya do�ru hareket et
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            // Dur
            rb.velocity = Vector2.zero;
        }

        // Oyuncu �ok yak�nsa sald�r
        if (distanceToPlayer <= attackRange && canAttack)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (player == null)
            return;

        // Sald�r�y� tetikle
        player.GetComponent<PlayerManager>().GetDamage(damage);
        canAttack = false;
        StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            GetDamage(other.GetComponent<BulletManager>().bulletDamage);
            Destroy(other.gameObject);
        }
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        slider.value = health;
        AmIDead();
    }

    void AmIDead()
    {
        if (health <= 0)
        {
            DataManager.Instance.EnemyKilled++;
            Destroy(gameObject);
        }
    }
}
