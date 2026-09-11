using UnityEngine;

public class Player_Health : Entity_Health
{
    private void Start()
    {
        Player.Instance.stats.OnUnlockedStatsChanged += UpdatedPersistentStats;
    }

    private void OnDisable()
    {
        Player.Instance.stats.OnUnlockedStatsChanged -= UpdatedPersistentStats;
    }
}