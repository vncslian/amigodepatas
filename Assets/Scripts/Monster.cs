using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour
{
    [Header("Entity Data")]
    public Entity entity;
    public GameManager manager;

    [Header("Chase / Combat")]
    public float chaseRange  = 6f;
    public float attackRange = 1.2f;

    [Header("Caca de Pets")]
    public float petDetectRange  = 8f;
    public float petCaptureRange = 0.8f;

    [Header("Passos")]
    public float stepInterval = 0.4f;

    [Header("Patrol")]
    public List<Transform> waypointList;
    public float arrivalDistance = 0.5f;
    public float waitTime        = 5f;
    public int   waypointID;

    [Header("Experience")]
    public int rewardExperience = 10;

    [Header("Respawn")]
    public GameObject prefab;
    public bool   respawn     = true;
    public float respawnTime = 10f;

    [Header("UI")]
    public Slider healthSlider;

    Rigidbody2D   rb2D;
    Animator      animator;
    Transform     player;
    PetController targetPet;
    PetController capturedPet;
    bool          combatCoroutineRunning = false;
    float         stepTimer             = 0f;

    Transform targetWaypoint;
    int       currentWaypoint    = 0;
    float     lastDistToWaypoint = 0f;
    float     currentWaitTime    = 0f;

    bool isMovingThisFrame = false;

    void Start()
    {
        rb2D     = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        manager  = GameManager.Instance;

        rb2D.gravityScale = 0f;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        entity.maxHealth  = manager.CalculateHealth(entity);
        entity.maxMana    = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);
        entity.currentHealth  = entity.maxHealth;
        entity.currentMana    = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        if (healthSlider != null) 
        { 
            healthSlider.maxValue = entity.maxHealth; 
            healthSlider.value = entity.maxHealth; 
        }

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            waypointList.Add(wp.transform);
        }
        
        currentWaitTime = waitTime;
        if (waypointList.Count > 0)
        {
            targetWaypoint     = waypointList[0];
            lastDistToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
        }
    }

    void Update()
    {
        if (entity.dead) return;
        if (entity.currentHealth <= 0) { Die(); return; }
        
        if (healthSlider) healthSlider.value = entity.currentHealth;
        
        if (entity.attackTimer > 0) entity.attackTimer -= Time.deltaTime;
        if (entity.attackTimer < 0) entity.attackTimer  = 0;

        isMovingThisFrame = false;

        DecideTarget();
        HandleStepSound();
    }

    void DecideTarget()
    {
        float distPlayer = player != null
            ? Vector2.Distance(transform.position, player.position)
            : float.MaxValue;

        if (distPlayer <= chaseRange)
        {
            targetPet = null;
            entity.inCombat = true;
            entity.target   = player.gameObject;

            GetComponent<MonsterAlert>()?.AtivarAlerta();

            if (distPlayer <= attackRange)
            {
                animator.SetBool("isWalking", false);
                StopMoving();
                if (!combatCoroutineRunning) StartCoroutine(AttackRoutine());
            }
            else
            {
                ChaseTransform(player);
                if (!combatCoroutineRunning) StartCoroutine(AttackRoutine());
            }
            return;
        }

        if (capturedPet == null)
        {
            PetController pet = FindNearestPet(petDetectRange);
            if (pet != null)
            {
                targetPet = pet;
                float distPet = Vector2.Distance(transform.position, pet.transform.position);
                if (distPet <= petCaptureRange) 
                { 
                    StopMoving(); 
                    animator.SetBool("isWalking", false); 
                    CapturarPet(pet); 
                }
                else 
                {
                    ChaseTransform(pet.transform);
                }
                return;
            }
        }

        entity.inCombat = false; entity.target = null; targetPet = null;
        if (waypointList.Count > 0) Patrol();
        else { animator.SetBool("isWalking", false); StopMoving(); }
    }

    PetController FindNearestPet(float range)
    {
        PetController nearest = null;
        float minDist = range;
        foreach (var pet in FindObjectsOfType<PetController>())
        {
            if (pet.isCaptured || pet.isTamed) continue;
            float d = Vector2.Distance(transform.position, pet.transform.position);
            if (d < minDist) { minDist = d; nearest = pet; }
        }
        return nearest;
    }

    void CapturarPet(PetController pet)
    {
        if (capturedPet != null) return;
        capturedPet = pet;
        pet.Capturar();
    }

    void ChaseTransform(Transform target)
    {
        if (!target) return;
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        
        animator.SetBool("isWalking", true);
        animator.SetFloat("input_x", dir.x);
        animator.SetFloat("input_y", dir.y);
        
        rb2D.MovePosition(rb2D.position + dir * entity.speed * Time.fixedDeltaTime);
        isMovingThisFrame = true;
    }

    void StopMoving() => rb2D.linearVelocity = Vector2.zero;

    void Patrol()
    {
        if (entity.dead || waypointList.Count == 0) return;
        float dist = Vector2.Distance(transform.position, targetWaypoint.position);
        
        if (dist <= arrivalDistance || dist >= lastDistToWaypoint)
        {
            animator.SetBool("isWalking", false);
            StopMoving();
            if (currentWaitTime <= 0)
            {
                currentWaypoint = (currentWaypoint + 1) % waypointList.Count;
                targetWaypoint     = waypointList[currentWaypoint];
                lastDistToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
                currentWaitTime    = waitTime;
            }
            else currentWaitTime -= Time.deltaTime;
        }
        else
        {
            animator.SetBool("isWalking", true);
            lastDistToWaypoint = dist;
            
            Vector2 d2 = ((Vector2)targetWaypoint.position - rb2D.position).normalized;
            animator.SetFloat("input_x", d2.x);
            animator.SetFloat("input_y", d2.y);
            
            rb2D.MovePosition(rb2D.position + d2 * entity.speed * Time.fixedDeltaTime);
            isMovingThisFrame = true;
        }
    }

    void HandleStepSound()
    {
        bool estaAndandoPelasAnimacoes = animator.GetBool("isWalking");

        if (!estaAndandoPelasAnimacoes)
        {
            stepTimer = 0f; 
            AudioManager.Instance?.PararPassoMonstro();
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f) 
        { 
            AudioManager.Instance?.TocarPassoMonstro(); 
            stepTimer = stepInterval; 
        }
    }

    public void ReceberDano(int dano)
    {
        if (entity.dead) return;
        
        entity.currentHealth -= dano;
        
        if (healthSlider != null)
        {
            healthSlider.value = entity.currentHealth;
        }

        AudioManager.Instance?.TocarHitMonstro();
        StartCoroutine(FlashVermelho());
        
        if (entity.currentHealth <= 0) Die();
    }

    IEnumerator FlashVermelho()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (!sr) yield break;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    IEnumerator AttackRoutine()
    {
        combatCoroutineRunning = true;
        while (entity.inCombat && !entity.dead)
        {
            yield return new WaitForSeconds(entity.cooldown);
            if (entity.target == null) break;
            
            var tp = entity.target.GetComponent<Player>();
            if (tp == null || tp.entity.dead) break;
            
            float dist = Vector2.Distance(transform.position, entity.target.transform.position);
            if (dist <= attackRange)
            {
                animator.SetTrigger("attack");
                AudioManager.Instance?.TocarAtaqueMonstro();
                
                int dmg     = manager.CalculateDamage(entity, entity.damage);
                int defense = manager.CalculateDefense(tp.entity, tp.entity.defense);
                int result  = Mathf.Max(0, dmg - defense);
                
                tp.entity.currentHealth -= result;
                AudioManager.Instance?.TocarDanoPlayer();
                CameraShake.Instance?.Shake(); 
            }
        }
        combatCoroutineRunning = false;
    }

    void Die()
    {
        if (entity.dead) return;

        GetComponent<MonsterAlert>()?.DesativarAlerta();

        entity.dead = true; entity.inCombat = false; entity.target = null;
        StopAllCoroutines();
        combatCoroutineRunning = false;
        
        if (healthSlider != null) healthSlider.value = 0f;

        animator.SetBool("isWalking", false);
        animator.SetTrigger("death");
        
        StopMoving();
        AudioManager.Instance?.PararPassoMonstro(); 
        
        rb2D.bodyType = RigidbodyType2D.Kinematic; 
        
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;

        if (capturedPet != null) { capturedPet.Libertar(); capturedPet = null; }

        AudioManager.Instance?.TocarMorteMonstro();
        
        if (manager != null) manager.OnMonsterKilled(entity.name);

        var pc = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        if (pc) pc.GainExp(rewardExperience);

        if (respawn) StartCoroutine(RespawnRoutine());
        else         Destroy(gameObject, 2f);
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        if (!prefab) yield break;
        
        var nm = Instantiate(prefab, transform.position, transform.rotation);
        nm.name = prefab.name;
        var m = nm.GetComponent<Monster>();
        if (m) { m.entity.dead = false; m.entity.combatCoroutine = false; }
        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !entity.dead) { entity.inCombat = true; entity.target = col.gameObject; }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) { entity.inCombat = false; entity.target = null; }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;     Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, petDetectRange);
        Gizmos.color = Color.yellow;  Gizmos.DrawWireSphere(transform.position, petCaptureRange);
    }
}