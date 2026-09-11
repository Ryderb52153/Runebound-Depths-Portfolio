using System.Collections.Generic;
using UnityEngine;

public class Skill_Object_TimeEcho : Skill_Object_Base, ITake_Damage
{
    // Registry of all echoes that are currently active and meant to distract
    // enemies. Enemies query this to prioritise attacking an echo over the player.
    private static readonly List<Skill_Object_TimeEcho> activeEchoes = new List<Skill_Object_TimeEcho>();
    public static IReadOnlyList<Skill_Object_TimeEcho> ActiveEchoes => activeEchoes;

    [SerializeField] private float wispMoveSpeed = 15;
    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask groundLayer;

    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;
    private Transform playerTransform;
    private bool shouldMoveToPlayer = false;
    private Entity_Health playerHealth;
    private Skill_Object_Health echoHealth;
    private Player_Skill_Manager playerSkillManager;
    private Entity_Status_Handler playerStatusHandler;

    public int maxAttacks { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        echoHealth = GetComponent<Skill_Object_Health>();
        wispTrail = GetComponentInChildren<TrailRenderer>();
        wispTrail.gameObject.SetActive(false);
    }

    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        playerStats = echoManager.player.stats;
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();
        playerTransform = echoManager.transform.root;
        playerHealth = echoManager.player.health;
        playerSkillManager = echoManager.player.skillManager;
        playerStatusHandler = echoManager.player.statusHandler;
        
        FlipToTarget();
        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration);
        anim.SetBool("canAttack", maxAttacks > 0);

        RegisterEcho();
    }

    private void RegisterEcho()
    {
        if (!activeEchoes.Contains(this))
            activeEchoes.Add(this);
    }

    private void UnregisterEcho()
    {
        activeEchoes.Remove(this);
    }

    private void Update()
    {
        if(shouldMoveToPlayer)
        {
            HandleWispMovement();
            return;
        }

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        StopHorizontalMovement();
    }

    public void HandleDeath()
    {
        CancelInvoke(nameof(HandleDeath));

        // No longer a valid distraction once it dies / turns into a wisp.
        UnregisterEcho();

        ObjectPoolManager.SpawnObject(onDeathVfx, transform.position, Quaternion.identity);

        if (echoManager.ShouldBeWisp())
            TurnIntoWisp();
        else
            ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true;
        anim.gameObject.SetActive(false);
        wispTrail.gameObject.SetActive(true);
        rb.simulated = false;
    }

    public void PerformAttack()
    {
        Collider2D[] enemies = GetEnemiesAround(targetCheck, 1);
        DamageEnemiesInRadius(enemies);
        CheckDuplicate();
    }

    private void CheckDuplicate()
    {
        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        float xOffset = transform.position.x < lastTarget?.position.x ? 1 : -1;

        if (canDuplicate)
            echoManager.CreateTimeEcho(transform.position + new Vector3(xOffset, 0, 0));
    }

    private void HandleWispMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, wispMoveSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            WispTouchedPlayer();
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void WispTouchedPlayer()
    {
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetPercentOfDamageHealed();
        playerHealth.IncreaseHealth(healAmount);

        float amountInSeconds = echoManager.GetcooldownReducedInSeconds();
        playerSkillManager.ReduceAllSkillCooldownBy(amountInSeconds);

        if(echoManager.CanRemoveNegativeEffects())
            playerStatusHandler.RemoveAllNegativeEffects();
    }

    private void FlipToTarget()
    {
        Transform target = ClosestTarget();

        if(target == null)
            return;

        if (target.position.x < transform.position.x)
            transform.Rotate(0,180, 0);
    }

    private void StopHorizontalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer);

        if(hit.collider != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnDisable()
    {
        UnregisterEcho();
        shouldMoveToPlayer = false;
        anim.gameObject.SetActive(true);
        wispTrail.gameObject.SetActive(false);
        rb.simulated = true;
    }

    public void TakeDamage(Attack_Data attackData, KnockbackData knockbackData)
    {
        HandleDeath();
    }
}
