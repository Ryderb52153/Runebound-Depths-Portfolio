using UnityEngine;

public class Enemy_Vampire_BatCastState : EnemyState
{
    private enum CastPhase { IdleToJump, FlyingUp, Casting, Floating, FallingDown, Complete }
    private CastPhase currentPhase;

    private VampireBat batPrefab;
    private Vector2 startPosition;
    private float riseHeight = 5f;
    private float riseSpeed = 8f;
    private float fallSpeed = 10f;
    private float floatDuration = 3f;

    private int numberOfBats = 5;
    private float batSpawnInterval = 0.1f;
    private float nextBatSpawnTime;
    private int batsSpawned;

    public Enemy_Vampire_BatCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName)
    {
        Enemy_Vampire enemyVamp = (Enemy_Vampire)enemy;
        batPrefab = enemyVamp.VampireBat;
    }

    public override void Enter()
    {
        base.Enter();

        startPosition = enemy.transform.position;
        currentPhase = CastPhase.IdleToJump;
        batsSpawned = 0;
    }

    public override void Update()
    {
        base.Update();

        switch (currentPhase)
        {
            case CastPhase.IdleToJump:
                UpdateIdleToJumpPhase();
                break;
            case CastPhase.FlyingUp:
                UpdateFlyingUpPhase();
                break;
            case CastPhase.Casting:
                UpdateCastingPhase();
                break;
            case CastPhase.Floating:
                UpdateFloatingPhase();
                break;
            case CastPhase.FallingDown:
                UpdateFallingDownPhase();
                break;
            case CastPhase.Complete:
                stateMachine.ChangeState(enemy.BattleState);
                break;
        }
    }

    private void UpdateIdleToJumpPhase()
    {
        enemy.SetVelocity(0, 0);

        if (CheckAnimationFinished("Vampire_To_Jump"))
            StartFlyingUp();
    }

    private void UpdateFlyingUpPhase()
    {
        enemy.SetVelocity(0, riseSpeed);

        float currentHeight = enemy.transform.position.y - startPosition.y;

        if (currentHeight >= riseHeight)
            StartCasting();
    }

    private void UpdateCastingPhase()
    {
        enemy.SetVelocity(0, 0);
        enemy.rb.simulated = false;

        if (Time.time >= nextBatSpawnTime && batsSpawned < numberOfBats)
        {
            SpawnBat();
            batsSpawned++;
            nextBatSpawnTime = Time.time + batSpawnInterval;
        }

        if (CheckAnimationFinished("Vampire_Bat_Cast"))
            StartFloating();
    }

    private void UpdateFloatingPhase()
    {
        enemy.SetVelocity(0, 0);

        if (stateTimer < 0)
            StartFalling();
    }

    private void UpdateFallingDownPhase()
    {
        enemy.SetVelocity(0, -fallSpeed);

        if (enemy.groundDetected)
            StartLanding();
    }

    private void StartFlyingUp()
    {
        currentPhase = CastPhase.FlyingUp;
        anim.SetBool("flying_up", true);
        anim.SetBool("idle_to_jump", false);
    }

    private void StartCasting()
    {
        currentPhase = CastPhase.Casting;
        nextBatSpawnTime = Time.time + batSpawnInterval;

        anim.SetBool("flying_up", false);
        anim.SetBool("vampire_cast", true);
    }

    private void StartFloating()
    {
        anim.SetBool("vampire_cast", false);
        stateTimer = floatDuration;
        currentPhase = CastPhase.Floating;
    }

    private void StartFalling()
    {
        enemy.rb.simulated = true;
        anim.SetBool("land", true);
        currentPhase = CastPhase.FallingDown;
    }

    private void StartLanding()
    {
        enemy.SetVelocity(0, 0);
        currentPhase = CastPhase.Complete;
        anim.SetBool("land", false);
    }

    private void SpawnBat()
    {
        if (batPrefab == null)
        {
            Debug.LogWarning("No bat prefab assigned to Bat Cast attack!");
            return;
        }

        Vector2 batSpawnPos = (Vector2)enemy.transform.position + new Vector2(0, 0.5f);
        Vector2 playerTargetPos = Player.Instance.transform.position;

        float spreadAngle = Random.Range(-15f, 15f);
        Vector2 spreadOffset = Quaternion.Euler(0, 0, spreadAngle) * (playerTargetPos - batSpawnPos).normalized * 2f;
        Vector2 finalTargetPos = playerTargetPos + spreadOffset;

        VampireBat batObject = ObjectPoolManager.SpawnObject(batPrefab, batSpawnPos, Quaternion.identity);

        if (batObject != null)
            batObject.Initialize(finalTargetPos, enemy.transform);
    }

    private bool CheckAnimationFinished(string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= 0.95f;
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("flying_up", false);
        enemy.rb.simulated = true;
        enemy.SetVelocity(0, 0);
    }
}