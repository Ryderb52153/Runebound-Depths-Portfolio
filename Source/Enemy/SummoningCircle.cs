using System.Collections;
using UnityEngine;

public class SummoningCircle : MonoBehaviour
{
    [Header("Summon Settings")]
    [SerializeField] private GameObject enemyPrefab; // Assign skeleton prefab
    [SerializeField] private float summonDelay = 1f; // Time before spawn
    [SerializeField] private float lifeDuration = 2f; // Total circle lifetime

    private void OnEnable()
    {
        StartCoroutine(SummonSequence());
    }

    private IEnumerator SummonSequence()
    {
        yield return new WaitForSeconds(summonDelay);

        SpawnEnemy();

        yield return new WaitForSeconds(lifeDuration - summonDelay);

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("No enemy prefab assigned to summoning circle!");
            return;
        }

        ObjectPoolManager.SpawnObject(enemyPrefab, transform.position, Quaternion.identity);
    }
}