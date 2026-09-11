

public class Enemy_Goblin : Enemy_Boss, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }

    protected override void Awake()
    {
        base.Awake();
        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_Goblin_AttackState(this, stateMachine, "attackBasic");
        BattleState = new Enemy_Boss_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "dead");
        StunnedState = new Enemy_Boss_StunState(this, stateMachine, "stunned");
        BattleMoveState = new Enemy_Boss_MoveBattleState(this, stateMachine, "isMoving");
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