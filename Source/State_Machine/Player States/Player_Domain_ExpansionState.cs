using UnityEngine;

public class Player_Domain_ExpansionState : PlayerState
{
    private Vector2 originalPosition;
    private float originalGravity;
    private float finalRiseDistance;

    private bool isLevitating;
    private bool createdDomain;


    public Player_Domain_ExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        originalPosition = player.transform.position;
        originalGravity = rb.gravityScale;
        finalRiseDistance = GetAvaliableRiseDistance();

        player.SetVelocity(0, player.riseSpeed);
        player.health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(originalPosition, player.transform.position) >= finalRiseDistance - 2.5 && isLevitating == false)
            Levitate();

        if(isLevitating)
        {
            skillManager.domainExpansion.DoSpellCasting();

            if (stateTimer < 0)
            {
                rb.gravityScale = originalGravity;
                isLevitating = false;
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        createdDomain = false;
        player.health.SetCanTakeDamage(true);
    }

    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        stateTimer = skillManager.domainExpansion.GetDomainDuration();

        if(createdDomain == false)
        {
            createdDomain = true;
        }
    }

    private float GetAvaliableRiseDistance()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.groundMask);

        return hit.collider != null ? hit.distance : player.riseMaxDistance;
    }
}