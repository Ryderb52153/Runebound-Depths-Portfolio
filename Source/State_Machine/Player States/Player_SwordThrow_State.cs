using UnityEngine;

public class Player_SwordThrow_State : PlayerState
{
    private Camera mainCamera;



    public Player_SwordThrow_State(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        //skillManager.swordThrow.EnableDots(true);

        if (mainCamera != Camera.main)
            mainCamera = Camera.main;
    }

    public override void Update()
    {
        base.Update();

        Vector2 directionToMouse = GetDirectionToMouse();

        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(directionToMouse.x);
        //skillManager.swordThrow.PredictTrajectory(directionToMouse);

        if (input.Player.Attack.WasPressedThisFrame())
        { 
            anim.SetBool("swordThrowPerformed", true);

            //skillManager.swordThrow.EnableDots(false);
            //skillManager.swordThrow.ConfirmTrajector(directionToMouse);
        }

        if(input.Player.Third_Ability.WasPressedThisFrame() || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    override public void Exit()
    {
        base.Exit();
        anim.SetBool("swordThrowPerformed", false);
        //skillManager.swordThrow.EnableDots(false);
    }

    private Vector2 GetDirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);
        Vector2 direction = (mousePosition - playerPosition).normalized;

        return direction;
    }
}
