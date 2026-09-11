using UnityEngine;

public class Skill_Object_AnimationTriggers : MonoBehaviour
{
    private Skill_Object_TimeEcho timeEcho;

    private void Awake()
    {
        timeEcho = GetComponentInParent<Skill_Object_TimeEcho>();
    }

    private void AttackTrigger()
    {
        timeEcho.PerformAttack();
    }

    private void TryTerminate(int currentAttackIndex)
    {
        if(currentAttackIndex == timeEcho.maxAttacks)
            timeEcho.HandleDeath();
    }
}
