using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = .1f;

    public float GetCounterRecoveryDuration() => counterRecovery;

    public bool CounterAttackPerformed()
    {
        bool hasCountered = false;

        foreach (var target in CircleattackColliders())
        {
            ICounterable counterableTarget = target.GetComponent<ICounterable>();

            if(counterableTarget == null)
                continue;

            if(counterableTarget.CanBeCountered)
            {
                counterableTarget.HandleCounterAttack();
                hasCountered = true;
            }
        }

        return hasCountered;
    }

    public Collider2D[] GetEnemiesBoxAttack()
    {
        return BoxAttackColliders();
    }
}