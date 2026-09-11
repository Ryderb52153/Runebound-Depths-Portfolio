
// Event hook that fires whenever the player uses ANY skill.
// Subscribes to every Skill_Base.OnSkillUsed via the Player_Skill_Manager.
public abstract class Item_Effect_OnSkillUsed : Item_Effect_DataSO
{
    public override void Subscribe(Player player)
    {
        base.Subscribe(player);

        if (this.player == null || this.player.skillManager == null)
            return;

        Skill_Base[] allSkills = this.player.skillManager.allSkills;
        if (allSkills == null)
            return;

        foreach (Skill_Base skill in allSkills)
        {
            if (skill != null)
                skill.OnSkillUsed += OnSkillUsed;
        }
    }

    public override void Unsubscribe()
    {
        if (player != null && player.skillManager != null && player.skillManager.allSkills != null)
        {
            foreach (Skill_Base skill in player.skillManager.allSkills)
            {
                if (skill != null)
                    skill.OnSkillUsed -= OnSkillUsed;
            }
        }

        base.Unsubscribe();
    }

    private void OnSkillUsed(Skill_Base skill)
    {
        TryUse();
    }
}
