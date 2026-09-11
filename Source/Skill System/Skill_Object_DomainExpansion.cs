using UnityEngine;

public class Skill_Object_DomainExpansion : Skill_Object_Base
{
    private Skill_Domain_Expansion domainManager;
    private float expandSpeed= 2f;
    private float slowDownPercent;
    private Vector3 targetScale;
    private float duration;
    private bool isShrinking;

    public void SetupDomain(Skill_Domain_Expansion domainManager)
    {
        this.domainManager = domainManager;
        float maxSize = domainManager.GetMaxDomainSize();
        duration = domainManager.GetDomainDuration();
        slowDownPercent = domainManager.GetSlowPercentage();
        expandSpeed = domainManager.GetExpandSpeed();

        targetScale = Vector3.one * maxSize;
        Invoke(nameof(ShrinkDown), duration);
    }

    private void Update()
    {
        HandleScaling();
    }

    private void HandleScaling()
    {
        float sizeDifference = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = sizeDifference > 0.1f;

        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * expandSpeed);

        if (isShrinking && sizeDifference < .1f)
        {
            TerminateDomain();
        }
    }

    private void TerminateDomain()
    {
        isShrinking = false;
        domainManager.ClearAllTargets();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void ShrinkDown()
    {
        targetScale = Vector3.zero;
        isShrinking = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Enemy>(out Enemy enemy);

        if (enemy == null)
            return;

        domainManager.AddTarget(enemy);
        enemy.SlowDownEntity(duration, slowDownPercent, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.TryGetComponent<Enemy>(out Enemy enemy);

        if (enemy != null)
            enemy.StopSlowDown();
    }
}