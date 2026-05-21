using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Player player;
    public Animator playerAnimator;
    float input_x = 0;
    float input_y = 0;
    bool isWalking = false;

    Rigidbody2D rb2D;
    Vector2 movement = Vector2.zero;

    [Header("Configuração de Passos")]
    public float stepInterval = 0.5f; 
    float stepTimer = 0f;

    void Start()
    {
        isWalking = false; 
        rb2D = GetComponent<Rigidbody2D>(); 
        player = GetComponent<Player>();
        
        rb2D.gravityScale = 0f;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");
        isWalking = (input_x != 0 || input_y != 0);
        movement = new Vector2(input_x, input_y);

        if (isWalking)
        {
            playerAnimator.SetFloat("input_x", input_x);
            playerAnimator.SetFloat("input_y", input_y);
            
            playerAnimator.ResetTrigger("attack");
        }

        playerAnimator.SetBool("isWalking", isWalking);

        HandleStepSound();

        if (player.entity.attackTimer < 0)
            player.entity.attackTimer = 0;
        else
            player.entity.attackTimer -= Time.deltaTime;

        if (!isWalking && rb2D.linearVelocity.magnitude < 0.01f && player.entity.attackTimer == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                playerAnimator.SetTrigger("attack");
                player.entity.attackTimer = player.entity.cooldown;

                Attack();
            }
        }         
    }
    
    private void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + movement.normalized * player.entity.speed * Time.fixedDeltaTime);
    }

    void HandleStepSound()
    {
        if (!isWalking)
        {
            stepTimer = 0f; 
            AudioManager.Instance?.PararPassoPlayer(); 
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            AudioManager.Instance?.TocarPassoPlayer();
            stepTimer = stepInterval;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Enemy")
        {
            player.entity.target = collider.transform.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.tag == "Enemy")
        {
            player.entity.target = null;
        }
    }

    void Attack()
    {
        if (isWalking || rb2D.linearVelocity.magnitude > 0.01f)
            return;

        if (player.entity.target == null)
            return;

        Monster monster = player.entity.target.GetComponent<Monster>();

        if (monster.entity.dead)
        {
            player.entity.target = null;
            return;
        }

        Vector2 posicaoPlayer2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 posicaoInimigo2D = new Vector2(player.entity.target.transform.position.x, player.entity.target.transform.position.y);

        float distance = Vector2.Distance(posicaoPlayer2D, posicaoInimigo2D);

        if(distance <= player.entity.attackDistance)
        {
            AudioManager.Instance?.TocarAtaquePlayer();

            int dmg = player.manager.CalculateDamage(player.entity, player.entity.damage);
            int enemyDef = player.manager.CalculateDefense(monster.entity, monster.entity.defense);
            int result = dmg - enemyDef;

            if (result < 0)
                result = 0;

            Debug.Log("Player dmg: " + result.ToString());
            
            monster.ReceberDano(result);
            monster.entity.target = this.gameObject;
        }
    }
}