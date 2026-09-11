using UnityEngine;

public class Enemy_Vampire : Enemy_Boss, ICounterable
{
    [SerializeField] private VampireBat vampireBatPrefab;

    public VampireBat VampireBat => vampireBatPrefab;

    private Enemy_Vampire_GrabState grabState;
    private Enemy_Vampire_BatCastState batCastState;

    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_Vampire_GrabState GrabState { get => grabState; set => grabState = value; }
    public Enemy_Vampire_BatCastState BatCastState { get => batCastState; set => batCastState = value; }

    protected override void Awake()
    {
        base.Awake();
        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_Vampire_AttackState(this, stateMachine, "attackBasic");
        BattleState = new Enemy_Boss_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "dead");
        StunnedState = new Enemy_Boss_StunState(this, stateMachine, "stunned");
        BattleMoveState = new Enemy_Boss_MoveBattleState(this, stateMachine, "isMoving");
        GrabState = new Enemy_Vampire_GrabState(this, stateMachine, "grab");
        BatCastState = new Enemy_Vampire_BatCastState(this, stateMachine, "idle_to_jump");
    }

    protected override void Start()
    {
        base.Start();
        Flip();
    }

    public void HandleCounterAttack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }
}