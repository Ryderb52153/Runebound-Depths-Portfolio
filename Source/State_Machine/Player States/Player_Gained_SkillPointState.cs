using UnityEngine;

public class Player_Gained_SkillPointState : PlayerState
{
    public Player_Gained_SkillPointState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0);
        player.DisableInput();
    }

    public override void Update()
    {
        if(triggerCalled)
        {
            triggerCalled = false; 
            player.GainSkillPoint();
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.EnableInput();
    }
}
