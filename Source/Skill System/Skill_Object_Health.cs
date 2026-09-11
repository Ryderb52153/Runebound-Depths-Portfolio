using UnityEngine;

public class Skill_Object_Health : Entity_Health
{
    protected override void Die()
    {
        Skill_Object_TimeEcho timeEcho = GetComponent<Skill_Object_TimeEcho>();
        timeEcho?.HandleDeath();
    }
}