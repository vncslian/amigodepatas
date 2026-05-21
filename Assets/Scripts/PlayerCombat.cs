using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    public float attackRange  = 1.5f;   
    public int   attackDamage = 20;     
    public float attackCooldown = 0.6f; 
    public LayerMask monsterLayer;      

    [Header("Morte e Respawn")]
    public GameObject    morteCanvas;   
    public float         respawnDelay = 4f;
    public Transform     respawnPoint; 

    [Header("Passos")]
    public float stepInterval = 0.3f;

    [Header("Efeito de Hit")]
    public GameObject hitFX;           

    Player         playerScript;
    Rigidbody2D    rb;
    Animator       anim;
    float          attackTimer = 0f;
    float          stepTimer   = 0f;
    bool           isDead      = false;
    Vector2        lastMoveDir = Vector2.down;

    void Start()
    {
        playerScript = GetComponent<Player>();
        rb           = GetComponent<Rigidbody2D>();
        anim         = GetComponent<Animator>();
        if (morteCanvas) morteCanvas.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        if (playerScript != null && playerScript.entity.currentHealth <= 0)
        {
            Morrer();
            return;
        }

        attackTimer -= Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            && attackTimer <= 0f)
        {
            Atacar();
            attackTimer = attackCooldown;
        }

        if (rb.linearVelocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                AudioManager.Instance?.TocarPassoPlayer();
                stepTimer = stepInterval;
            }
            lastMoveDir = rb.linearVelocity.normalized;
        }
        else stepTimer = 0f;
    }

    void Atacar()
    {
        AudioManager.Instance?.TocarAtaquePlayer();

        if (anim != null)
        {
            if (HasParam("attack")) anim.SetTrigger("attack");
        }

        Vector2 origem = (Vector2)transform.position + lastMoveDir * 0.5f;
        Collider2D[] alvos = Physics2D.OverlapCircleAll(origem, attackRange, monsterLayer);

        foreach (var col in alvos)
        {
            var monster = col.GetComponent<Monster>();
            if (monster == null) monster = col.GetComponentInParent<Monster>();
            if (monster == null || monster.entity.dead) continue;

            int dano = attackDamage;
            if (playerScript != null)
                dano = GameManager.Instance != null
                    ? GameManager.Instance.CalculateDamage(playerScript.entity, attackDamage)
                    : attackDamage;

            monster.ReceberDano(dano);
            AudioManager.Instance?.TocarHitPlayer();

            if (hitFX != null)
            {
                var fx = Instantiate(hitFX, col.transform.position, Quaternion.identity);
                Destroy(fx, 0.5f);
            }
        }
    }

    void Morrer()
    {
        if (isDead) return;
        isDead = true;

        AudioManager.Instance?.TocarMortePlayer();
        AudioManager.Instance?.PararMusica();

        if (anim != null && HasParam("death")) anim.SetTrigger("death");

        rb.linearVelocity = Vector2.zero;
        foreach (var col in GetComponents<Collider2D>()) col.enabled = false;

        if (morteCanvas) morteCanvas.SetActive(true);

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        isDead = false;
        if (playerScript != null)
        {
            playerScript.entity.currentHealth = playerScript.entity.maxHealth;
            playerScript.entity.dead = false;
        }


        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        foreach (var col in GetComponents<Collider2D>()) col.enabled = true;
        if (morteCanvas) morteCanvas.SetActive(false);

        AudioManager.Instance?.TocarMusica();

        if (anim != null && HasParam("isWalking")) anim.SetBool("isWalking", false);
    }

    bool HasParam(string name)
    {
        foreach (var p in anim.parameters)
            if (p.name == name) return true;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + lastMoveDir * 0.5f, attackRange);
    }
}
