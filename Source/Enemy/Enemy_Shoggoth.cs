
public class Enemy_Shoggoth : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        BattleMoveState = new Enemy_Battle_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "dead");
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(IdleState);
    }

    public void HandleCounterAttack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }
}