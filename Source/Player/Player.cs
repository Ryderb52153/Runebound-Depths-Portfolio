using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static Player Instance { get; private set; }

    public static event Action OnPlayerDeath;
    public static event Action OnPlayerBasicAttack;
    public static event Action OnPlayerFootstep;
    public event Action PlayerGainedSkillPoint;

    private UI_Manager uiManager;
    public Player_Skill_Manager skillManager { get; private set; }
    public Inventory_Player inventory { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Player_Stats stats { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_Status_Handler statusHandler { get; private set; }
    public Player_Combat combat { get; private set; }
    public PlayerInputSet inputSet { get; private set; }

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_Wall_SlideState wallSlideState { get; private set; }
    public Player_Wall_JumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_Basic_AttackState basicAttackState { get; private set; }
    public Player_Jump_AttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_Counter_AttackState counterAttackState { get; private set; }
    public Player_Domain_ExpansionState domainExpansionState { get; private set; }
    public Player_ClimbState climbState { get; private set; }
    public Player_Climb_IdleState climbIdleState { get; private set; }
    public Player_SwordSlashState swordSlashState { get; private set; }
    public Player_Ledge_GrabState ledgeGrabState { get; private set; }
    public Player_Ledge_ClimbingState ledgeClimbState { get; private set; }
    public Player_Gained_SkillPointState skillPointGained { get; private set; }
    public Player_GrabbedState grabbedState { get; private set; }
    public Player_WallHitState wallHitState { get; private set; }


    [SerializeField] private Transform ledgeCheckTransform;

    [Header("Ultimate ability details")]
    public float riseSpeed = 25;
    public float riseMaxDistance = 3;

    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;

    [Header("Movement Details")]
    public float moveSpeed = 5;
    public float jumpForce = 12;
    public float idleJumpForce = 5;
    public Vector2 wallJumpForce;
    public float climbSpeed = 5;
    [Range(0, 1)]
    public float inAirMoveMultiplier;
    [Range(0, 1)]
    public float wallSlideMultiplier;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20f;

    public Vector2 movementInput { get; private set; }
    public Vector2 mousePosition { get; private set; }

    private bool isDroppingDown = false;
    private Coroutine dropDownCo;
    private int ladderLayer;

    public Collider2D currentLadder { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        base.Awake();

        ladderLayer = LayerMask.NameToLayer("Ladder");

        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        stats = GetComponent<Player_Stats>();
        uiManager = FindAnyObjectByType<UI_Manager>();
        skillManager= GetComponent<Player_Skill_Manager>();
        statusHandler = GetComponent<Entity_Status_Handler>();
        combat = GetComponent<Player_Combat>();
        inventory = GetComponent<Inventory_Player>();

        inputSet = new PlayerInputSet();
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_Wall_SlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_Wall_JumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_Basic_AttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_Jump_AttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttackState = new Player_Counter_AttackState(this, stateMachine, "counterAttack");
        domainExpansionState = new Player_Domain_ExpansionState(this, stateMachine, "jumpFall");
        climbState = new Player_ClimbState(this, stateMachine, "climb");
        climbIdleState = new Player_Climb_IdleState(this, stateMachine, "climbIdle");
        swordSlashState = new Player_SwordSlashState(this, stateMachine, "swordSlashBegin");
        ledgeGrabState = new Player_Ledge_GrabState(this, stateMachine, "ledgeGrab");
        ledgeClimbState = new Player_Ledge_ClimbingState(this, stateMachine, "ledgeClimb");
        skillPointGained = new Player_Gained_SkillPointState(this, stateMachine, "gainedSkillPoint");
        grabbedState = new Player_GrabbedState(this, stateMachine, "isGrabbed");
        wallHitState = new Player_WallHitState(this, stateMachine, "wallHit");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void DisableInput() => inputSet.Disable();   
    public void EnableInput() => inputSet.Enable();

    public void RaiseBasicAttack() => OnPlayerBasicAttack?.Invoke();
    public void RaiseFootstep() => OnPlayerFootstep?.Invoke();

    public void SetCombatInputActive(bool active)
    {
        if (active)
        {
            inputSet.Player.Jump.Enable();
            inputSet.Player.Attack.Enable();
            inputSet.Player.CounterAttack.Enable();
            inputSet.Player.First_Ability.Enable();
            inputSet.Player.Second_Ability.Enable();
            inputSet.Player.Third_Ability.Enable();
            inputSet.Player.Ultimate_Ability.Enable();
        }
        else
        {
            inputSet.Player.Jump.Disable();
            inputSet.Player.Attack.Disable();
            inputSet.Player.CounterAttack.Disable();
            inputSet.Player.First_Ability.Disable();
            inputSet.Player.Second_Ability.Disable();
            inputSet.Player.Third_Ability.Disable();
            inputSet.Player.Ultimate_Ability.Disable();
        }
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void SwitchToSkillPointState()
    {
        StartCoroutine(GainedSkillPointCo());
    }

    public void SwitchToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }

    public void SwitchToThrowState()
    {
        stateMachine.ChangeState(wallHitState);
    }

    public void SwitchToGrabState()
    {
        stateMachine.ChangeState(grabbedState);
    }

    private IEnumerator GainedSkillPointCo()
    {
        while (!groundDetected)
        {
            yield return new WaitForFixedUpdate();
        }

        stateMachine.ChangeState(skillPointGained);
    }

    public void GainSkillPoint()
    {
        PlayerGainedSkillPoint?.Invoke();
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttackVelocity = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = new Vector2[attackVelocity.Length];
        Array.Copy(attackVelocity, originalAttackVelocity, attackVelocity.Length);


        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        anim.speed *= speedMultiplier;
        wallJumpForce *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJump;
        jumpAttackVelocity = originalJumpAttackVelocity;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    public void TeleportPlayer(Vector3 position) => transform.position = position;

    public override void EntityDeath()
    {
        base.EntityDeath();
        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    public void DropDownPlatform()
    {
        if (isDroppingDown)
            return;

        if (dropDownCo != null)
            StopCoroutine(dropDownCo);

        isDroppingDown = true;
        dropDownCo = StartCoroutine(DropDownPlatformCo());
    }

    private void ChangePlayerLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    private IEnumerator DropDownPlatformCo()
    {
        ChangePlayerLayer("PlayerFallThruPlatform");
        yield return new WaitForSeconds(.45f);
        isDroppingDown = false;
        ChangePlayerLayer("Player");
    }

    private void Interact()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (Collider2D target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();
            if(interactable == null)
                continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target.transform;
            }
        }

        if (closest == null)
            return;

        closest.GetComponent<IInteractable>().Interact();
    }

    protected override void Update()
    {
        base.Update();

        if (currentLadder != null && !isClimbing())
        {
            if (Mathf.Abs(movementInput.y) > 0.1f)
            {
                StartClimbing(currentLadder);
            }
        }
    }

    private bool isClimbing() => stateMachine.currentState == climbState || stateMachine.currentState == climbIdleState;

    private void StartClimbing(Collider2D collision)
    {
        stateMachine.ChangeState(climbIdleState);
    }

    public bool CheckForLedge()
    {
        if (!wallDetected) return false;

        Vector2 direction = new Vector2(facingDirection, 0);
        bool ceilingHit = Physics2D.OverlapBox(ledgeCheckTransform.position + Vector3.up * 0.5f, new Vector2(0.5f, 0.5f), 0, groundMask);

        if (ceilingHit) return false;

        RaycastHit2D wallHit = Physics2D.Raycast(_primaryWallCheck.position, Vector2.right * facingDirection, _wallCheckDistance, groundMask);
        Vector2 downCastStart = wallHit.point + Vector2.up * 1.0f + direction * 0.05f;
        RaycastHit2D downCastHit = Physics2D.Raycast(downCastStart, Vector2.down, 1.5f, groundMask);

        if (downCastHit)
        {
            Vector2 wallPoint = wallHit.point;
            Vector2 topPoint = downCastHit.point;

            float hangOffsetX = 0.25f;
            float hangOffsetY = 0.6f;

            ledgeClimbState.LedgePoint = new Vector2(
                wallPoint.x - (facingDirection * hangOffsetX),
                topPoint.y - hangOffsetY
            );

            return true;
        }

        return false;
    }

    public bool CheckForWallWhileThrown()
    {
        Vector2 direction = new Vector2(-facingDirection, 0);
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, .6f, groundMask);
        return wallHit.collider != null;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(ledgeCheckTransform.position.x, ledgeCheckTransform.position.y, 0), new Vector3(0.5f, 0.5f, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(ledgeCheckTransform.position.x + Vector2.up.x, ledgeCheckTransform.position.y + 0.5f, 0), new Vector3(0.5f, 0.5f, 1));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == ladderLayer)
        {
            currentLadder = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == ladderLayer)
        {
            currentLadder = null;

            if (stateMachine.currentState == climbState || stateMachine.currentState == climbIdleState)
                stateMachine.ChangeState(fallState);
        }
    }

    protected override void OnEnable()
    {
        inputSet.Enable();

        inputSet.Player.Mouse.performed += context => mousePosition = context.ReadValue<Vector2>();
        inputSet.Player.Movement.performed += context => movementInput = context.ReadValue<Vector2>();
        inputSet.Player.Movement.canceled += context => movementInput = Vector2.zero;
        inputSet.Player.Interact.performed += context => Interact();
        inputSet.Player.QuickItem.performed += context => inventory.TryUseQuickItem();

        inputSet.Player.ToggleInventoryUI.performed += context => uiManager.ToggleInventoryUI();
        inputSet.Player.OptionsWindow.performed += context => uiManager.ToggleOptionsWindow();
    }

    private void OnDisable()
    {
        inputSet.Disable();

        inputSet.Player.Mouse.performed -= context => mousePosition = context.ReadValue<Vector2>();

        inputSet.Player.Movement.performed -= context => movementInput = context.ReadValue<Vector2>();
        inputSet.Player.Movement.canceled -= context => movementInput = Vector2.zero;
        inputSet.Player.Interact.performed -= context => Interact();
        inputSet.Player.QuickItem.performed -= context => inventory.TryUseQuickItem();

        inputSet.Player.ToggleInventoryUI.performed -= context => uiManager.ToggleInventoryUI();
        inputSet.Player.OptionsWindow.performed -= context => uiManager.ToggleOptionsWindow();
    }
}
