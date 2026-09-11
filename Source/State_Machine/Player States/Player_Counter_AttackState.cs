using UnityEngine;

public class Player_Counter_AttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSomebody;

    public Player_Counter_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = combat.GetCounterRecoveryDuration();
        counteredSomebody = combat.CounterAttackPerformed();
        anim.SetBool("counterAttackPerformed", counteredSomebody);
    }

    public override void Update()
    {
        base.Update();

        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0 && counteredSomebody == false)
            stateMachine.ChangeState(player.idleState);
    }
}