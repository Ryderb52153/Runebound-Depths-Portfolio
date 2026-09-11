using UnityEngine;

public class Skill_Dash : Skill_Base
{
    public override void UseSkill()
    {
        base.UseSkill();
        OnStartEffect();
    }

    private void OnStartEffect()
    {
        if(Unlocked(Skill_Upgrade_Type.Dash_CloneOnStart) || Unlocked(Skill_Upgrade_Type.Dash_CloneOnStartAndArrival))
            CreateClone();
        else if(Unlocked(Skill_Upgrade_Type.Dash_ShardOnStart) || Unlocked(Skill_Upgrade_Type.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    public override void OnEndEffect()
    {
        if (Unlocked(Skill_Upgrade_Type.Dash_CloneOnStartAndArrival))
            CreateClone();

        if (Unlocked(Skill_Upgrade_Type.Dash_ShardOnStartAndArrival))
            CreateShard();
    }

    private void CreateShard()
    {
        skill_Manager.shard.CreateRawShard();
    }

    private void CreateClone()
    {
        skill_Manager.timeEcho.CreateTimeEcho();
    }
}