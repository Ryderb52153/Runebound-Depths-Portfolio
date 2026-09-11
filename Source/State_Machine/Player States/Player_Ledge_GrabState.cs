
public class Player_Ledge_GrabState : PlayerState
{
    public Player_Ledge_GrabState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0);
        player.rb.gravityScale = 0;
    }

    public override void Update()
    {
        if (player.movementInput.y > .5f)
            stateMachine.ChangeState(player.ledgeClimbState);
        else if (input.Player.Jump.WasPressedThisFrame())
        {
            player.Flip();
            stateMachine.ChangeState(player.jumpState);
        }
        else if(player.movementInput.y < -.5f)
        {
            player.Flip();
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = 3.5f;
    }
}