using System;
using Unity.Cinemachine;
using UnityEngine;

public class Boss_Fight : MonoBehaviour
{
    [SerializeField] private Enemy_Boss bossPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineCamera bossCamera;

    public static event Action<Enemy_Boss, CinemachineCamera> OnBossFightStarted;
    
    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Enemy_Boss boss = ObjectPoolManager.SpawnObject<Enemy_Boss>(bossPrefab, spawnPoint.transform.position, Quaternion.identity);
            boss.gameObject.SetActive(false);
            OnBossFightStarted?.Invoke(boss, bossCamera);
            triggerCollider.enabled = false;
        }
    }
}
