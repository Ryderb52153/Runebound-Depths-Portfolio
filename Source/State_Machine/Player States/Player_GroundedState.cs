using UnityEngine;

public class Player_GroundedState : PlayerState
{
    private float _coyoteTime = .1f;

    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        if(player.groundDetected == false)
            _coyoteTime -= Time.deltaTime;
        else
            _coyoteTime = .1f;

        if (rb.linearVelocity.y < 0 && _coyoteTime < 0)
            stateMachine.ChangeState(player.fallState);

        if(input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);

        if(input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);

        if(input.Player.CounterAttack.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttackState);

    }
}