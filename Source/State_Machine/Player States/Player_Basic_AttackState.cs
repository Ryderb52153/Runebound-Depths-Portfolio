using UnityEngine;

public class Player_Basic_AttackState : PlayerState
{
    private float attackVelocityTimer;

    private const int FIRSTCOMBOINDEX = 1;
    private int attackDirection;
    private bool comboAttackQueued;
    private int comboIndex = 1;
    private int comboMax = 3;
    private float lastTimeAttacked;


    public Player_Basic_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboMax != player.attackVelocity.Length)
            comboMax = player.attackVelocity.Length;
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        CheckComboIndex();
        SyncAttackSpeed();

        attackDirection = player.movementInput.x != 0 ? (int)player.movementInput.x : player.facingDirection;

        anim.SetInteger("basicAttackIndex", comboIndex);
        GenerateAttackVelocity();

        Player.Instance.RaiseBasicAttack();
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
            HandleStateExit();
    }

    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
    }


    private void QueueNextAttack()
    {
        if(comboIndex < comboMax)
        {
            comboAttackQueued = true;
        }
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();
        }
        else
            stateMachine.ChangeState(player.idleState);
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);
    }

    private void GenerateAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDirection, attackVelocity.y);
    }

    private void CheckComboIndex()
    {
        if(Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FIRSTCOMBOINDEX;

        if (comboIndex > comboMax)
            comboIndex = FIRSTCOMBOINDEX;
    }
}