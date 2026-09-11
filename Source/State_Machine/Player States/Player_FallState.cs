using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    override public void Enter()
    {
        base.Enter();
        rb.gravityScale = 4;
    }

    public override void Update()
    {
        base.Update();

        if(player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = 3.5f;
    }
}