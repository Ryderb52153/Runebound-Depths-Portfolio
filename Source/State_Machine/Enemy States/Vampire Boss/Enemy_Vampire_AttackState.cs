using UnityEngine;

public class Enemy_Vampire_AttackState : Enemy_AttackState
{
    private AttackConfig currentAttack;

    public Enemy_Vampire_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        currentAttack = enemy.currentAttack;
        anim.SetBool(currentAttack.animationTrigger, true);
        triggerCalled = false;
        SyncAttackSpeed();

        Enemy_Vampire vampire = (Enemy_Vampire)enemy;

        if (currentAttack.attackName == "Grab")
        {
            stateMachine.ChangeState(vampire.GrabState);
            return;
        }
        else if (currentAttack.attackName == "Bat Cast")
        {
            stateMachine.ChangeState(vampire.BatCastState);
            return;
        }
        else if (currentAttack.attackName == "Summon Adds")
        {
            if(currentAttack.prefab == null)
                return;

            Vector2 spawnPosition = (Vector2)enemy.transform.position + new Vector2(enemy.facingDirection * 3f, 0);
            RaycastHit2D groundHit = Physics2D.Raycast(spawnPosition, Vector2.down, 10f, enemy.groundMask);
            
            if (groundHit.collider != null)
                spawnPosition.y = groundHit.point.y + .75f;

            ObjectPoolManager.SpawnObject(currentAttack.prefab, spawnPosition, Quaternion.identity);
        }       
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)  
            stateMachine.ChangeState(enemy.BattleState);
    }

    public override void Exit() 
    {
        anim.SetBool(currentAttack.animationTrigger, false);
    }
}