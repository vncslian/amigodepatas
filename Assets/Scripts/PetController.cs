using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PetController : MonoBehaviour
{
    public enum PetState { Idle, Walking, Eating, Sleeping, Following, Captured }
    public enum PetType  { Dog, Rusk, Cat }

    [Header("Tipo")]
    public PetType petType = PetType.Cat;

    [Header("Wandering")]
    public float moveSpeed    = 2f;
    public float wanderRadius = 5f;

    [Header("Adestramento")]
    public int   treatsNeeded   = 3;
    public float interactRadius = 1.2f;

    [Header("Seguir")]
    public float followSpeed = 3f;
    public float stopDist    = 0.7f;

    [Header("Efeitos Visuais")]
    public GameObject tamedFX;
    public GameObject capturedFX;
    public GameObject treatParticleFX;   
    public Color capturedColor = new Color(0.4f, 0.6f, 1f, 1f);

    [Header("Passos")]
    public float stepInterval = 0.35f;   

    [HideInInspector] public bool isTamed    = false;
    [HideInInspector] public bool isCaptured = false;

    Rigidbody2D    rb;
    Animator       anim;
    SpriteRenderer sr;
    Transform      player;
    PetState       state           = PetState.Idle;
    Vector2        moveDir         = Vector2.zero;
    Vector2        startPos;
    float          waitTimer       = 0f;
    int            treatsGiven     = 0;
    Vector2        wanderTarget;
    bool           hasWanderTarget = false;
    GameObject     capturedFXInst;
    float          stepTimer       = 0f;

    bool usesCatParams = false;
    bool usesDogParams = false;

    void Start()
    {
        rb       = GetComponent<Rigidbody2D>();
        anim     = GetComponent<Animator>();
        sr       = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0f;
        DetectAnimatorType();
        FindPlayer();
        ForceWalk();
    }

    void DetectAnimatorType()
    {
        foreach (var p in anim.parameters)
        {
            if (p.name == "moveX")   { usesCatParams = true; return; }
            if (p.name == "input_x") { usesDogParams = true; return; }
        }
    }

    void Update()
    {
        if (isCaptured) return;
        if (player == null) FindPlayer();

        if (isTamed) HandleFollow();
        else { HandleWander(); CheckTreatInput(); }

        HandleStepSound();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isCaptured) { rb.linearVelocity = Vector2.zero; return; }
        if (state == PetState.Walking || state == PetState.Following)
        {
            float spd = isTamed ? followSpeed : moveSpeed;
            rb.MovePosition(rb.position + moveDir * spd * Time.fixedDeltaTime);
        }
        else rb.linearVelocity = Vector2.zero;
    }

    void FindPlayer()
    {
        var p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;
    }

    void HandleStepSound()
    {
        bool moving = state == PetState.Walking || state == PetState.Following;
        
        if (!moving || isCaptured) 
        { 
            stepTimer = 0f; 
            AudioManager.Instance?.PararPassoPet(); 
            return; 
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            if (petType == PetType.Cat)
            {
                AudioManager.Instance?.TocarPassoGato();
            }
            else
            {
                AudioManager.Instance?.TocarPassoCachorro();
            }
            stepTimer = stepInterval;
        }
    }

    void HandleWander()
    {
        waitTimer -= Time.deltaTime;
        if (state == PetState.Walking && hasWanderTarget)
        {
            if (Vector2.Distance(transform.position, wanderTarget) < 0.4f)
            { ScheduleNextAction(); return; }
            moveDir = (wanderTarget - (Vector2)transform.position).normalized;
        }
        if (waitTimer <= 0f) ScheduleNextAction();
    }

    void ScheduleNextAction()
    {
        if (Random.Range(0,3) == 0)
        {
            if (usesDogParams) SetState(PetState.Idle, Random.Range(2f,5f));
            else SetState(Random.value > 0.5f ? PetState.Sleeping : PetState.Eating, Random.Range(2f,5f));
            moveDir = Vector2.zero; hasWanderTarget = false;
        }
        else ForceWalk();
    }

    void ForceWalk()
    {
        wanderTarget    = startPos + Random.insideUnitCircle * wanderRadius;
        hasWanderTarget = true;
        SetState(PetState.Walking, Random.Range(3f,6f));
        moveDir = (wanderTarget - (Vector2)transform.position).normalized;
    }

    void SetState(PetState s, float d) { state = s; waitTimer = d; }

    void CheckTreatInput()
    {
        if (player == null) return;
        if (Vector2.Distance(transform.position, player.position) > interactRadius) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        var inv = player.GetComponent<PlayerInventory>();
        if (inv == null || !inv.UseTreat()) return;

        treatsGiven++;
        AudioManager.Instance?.TocarTreat();

        if (treatParticleFX != null)
        {
            var fx = Instantiate(treatParticleFX, transform.position + Vector3.up*0.5f, Quaternion.identity);
            Destroy(fx, 1.5f);
        }
        StartCoroutine(FlashSprite(Color.yellow));
        if (treatsGiven >= treatsNeeded) TamePet();
    }

    void TamePet()
    {
        isTamed = true; state = PetState.Following;
        hasWanderTarget = false; moveDir = Vector2.zero;
        if (player == null) FindPlayer();
        if (tamedFX != null) Instantiate(tamedFX, transform.position, Quaternion.identity);
        GameManager.Instance?.OnPetTamed(petType.ToString().ToLower());
        StartCoroutine(FlashSprite(Color.green));
    }

    void HandleFollow()
    {
        if (player == null) return;
        float d = Vector2.Distance(transform.position, player.position);
        if (d > stopDist) { moveDir = ((Vector2)player.position - rb.position).normalized; state = PetState.Following; }
        else              { moveDir = Vector2.zero; state = PetState.Idle; }
    }

    public void Capturar()
    {
        if (isCaptured || isTamed) return;
        isCaptured = true; state = PetState.Captured; moveDir = Vector2.zero;
        if (sr) sr.color = capturedColor;

        if (capturedFX != null)
            capturedFXInst = Instantiate(capturedFX, transform.position + Vector3.up*0.4f,
                                         Quaternion.identity, transform);

        AudioManager.Instance?.PararPassoPet();

        if (petType == PetType.Cat)
        {
            AudioManager.Instance?.TocarGatoCapturado();
        }
        else
        {
            AudioManager.Instance?.TocarCachorroCapturado();
        }

        StartCoroutine(ShakeOnCapture());
        HUD.Instance?.RegistrarPetCapturado();
        UpdateAnimator();
    }

    public void Libertar()
    {
        if (!isCaptured) return;
        isCaptured = false;
        if (capturedFXInst) Destroy(capturedFXInst);
        if (sr) sr.color = Color.white;
        AudioManager.Instance?.TocarPetLibertado();
        HUD.Instance?.RegistrarPetLibertado();
        StartCoroutine(FlashSprite(Color.cyan));
        ForceWalk();
    }

    IEnumerator ShakeOnCapture()
    {
        Vector3 orig = transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            transform.localPosition = orig + new Vector3(Random.Range(-0.08f, 0.08f), Random.Range(-0.08f, 0.08f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        transform.localPosition = orig;
    }

    IEnumerator FlashSprite(Color c)
    {
        if (!sr) yield break;
        sr.color = c;
        yield return new WaitForSeconds(0.25f);
        if (!isCaptured) sr.color = Color.white;
    }

    void UpdateAnimator()
    {
        bool moving   = state == PetState.Walking || state == PetState.Following;
        bool eating   = state == PetState.Eating;
        bool sleeping = state == PetState.Sleeping;
        if (usesCatParams)
        {
            anim.SetFloat("moveX",      moveDir.x);
            anim.SetFloat("moveY",      moveDir.y);
            anim.SetBool ("isMoving",   moving);
            anim.SetBool ("isEating",   eating);
            anim.SetBool ("isSleeping", sleeping);
        }
        else if (usesDogParams)
        {
            anim.SetFloat("input_x",   moveDir.x);
            anim.SetFloat("input_y",   moveDir.y);
            anim.SetBool ("isWalking", moving);
            anim.SetFloat("speed",     moving ? 1f : 0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)startPos : transform.position, wanderRadius);
    }
}