using System.Collections;
using UnityEngine;

public class Entity_Status_Handler : MonoBehaviour
{
    [Header("Shock effect details")]
    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maxCharge = 1;
    private Coroutine shockCo;

    private ElementType currentEffect = ElementType.None;
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
    }

    public void ApplyStatusEffect(ElementType element, Elemental_Effect_Data effectData)
    {
        if(element == ElementType.Ice && CanBeApplied(ElementType.Ice))
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowEffect);

        if(element == ElementType.Fire && CanBeApplied(ElementType.Fire))
            ApplyBurnEffect(effectData.burnDuration, effectData.totalBurnDamae);

        if(element == ElementType.Lightning && CanBeApplied(ElementType.Lightning))
            ApplyShockEffect(effectData.shockDuration, effectData.totalShockDamage, effectData.shockCharge);
    }

    public bool CanBeApplied(ElementType element)
    {
        if (element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;

        return currentEffect == ElementType.None;
    }

    public void RemoveAllNegativeEffects()
    {
        StopAllCoroutines();
        currentEffect = ElementType.None;
        entityVfx.StopAllVfx();
    }

    private void ApplyShockEffect(float duration, float damage, float charge)
    {
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);

        currentCharge += finalCharge;

        if (currentCharge >= maxCharge)
        {
            SpawnLightningStrike(damage);
            StopShockEffect();
            return;
        }

        if (shockCo != null)
            StopCoroutine(shockCo);

        shockCo = StartCoroutine(ShockEffectCo(duration));
    }

    private void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration = duration * (1 - iceResistance);
        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));
    }

    private void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1 - fireResistance);
        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }

    private void SpawnLightningStrike(float damage)
    {
        ObjectPoolManager.SpawnObject(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage, false);
        UI_FloatingText_Manager.Instance.CreateFloatingText(damage.ToString(), transform.position, TextType.Shock);
    }

    private void StopShockEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        entityVfx.StopAllVfx(); 
    }

    private IEnumerator ShockEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Lightning);
        yield return new WaitForSeconds(duration);
        StopShockEffect();
    }

    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);

        float damagePerTick = totalDamage / tickCount;
        float tickInterval = 1f / ticksPerSecond;

        for (int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick, false);
            UI_FloatingText_Manager.Instance.CreateFloatingText(damagePerTick.ToString(), transform.position, TextType.Burning);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentEffect = ElementType.None;
        entityVfx.StopAllVfx();
    }
}