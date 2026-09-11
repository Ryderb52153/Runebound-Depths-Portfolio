using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    [Header("On Taken Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVFXDuration = 0.2f;

    [Header("On Doing Damage VFX")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;

    protected SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Entity entity;
    private Entity_Health entityHealth;

    private Coroutine damageVfxCo;
    private Coroutine statusVfxCo;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        entity = GetComponent<Entity>();
        entityHealth = GetComponent<Entity_Health>();
    }

    private void OnEnable()
    {
        entityHealth.OnTakenDamage += PlayOnDamageVFX;
    }

    private void OnDisable()
    {
        entityHealth.OnTakenDamage -= PlayOnDamageVFX;
    }

    public void CreateOnHitVfx(bool isCrit)
    {
        GameObject vfxPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject spawnedVfx = ObjectPoolManager.SpawnObject(vfxPrefab, transform.position, Quaternion.identity);

        if (entity.facingDirection == -1 && isCrit && spawnedVfx != null)
            spawnedVfx.transform.Rotate(0, 180, 0);
    }

    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Ice:
                return chillVfx;
            case ElementType.Fire:
                return burnVfx;
            case ElementType.Lightning:
                return shockVfx;
            default:
                return Color.white;
        }
    }

    public void PlayOnDamageVFX(Attack_Data attackData)
    {
        if(attackData == null) return;
        if(attackData.vfxOverride) return;

        CreateOnHitVfx(attackData.isCrit);

        if (damageVfxCo != null)
            StopCoroutine(damageVfxCo);

        damageVfxCo = StartCoroutine(PlayOnDamageVFXCo());
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        Color color = GetElementColor(element);

        if(statusVfxCo != null)
            StopCoroutine(statusVfxCo);

        statusVfxCo = StartCoroutine(PlayStatusVfxColor(duration, color));
    }

    public void StopAllVfx()
    {
        if (damageVfxCo != null)
        {
            StopCoroutine(damageVfxCo);
            damageVfxCo = null;
        }

        if (statusVfxCo != null)
        {
            StopCoroutine(statusVfxCo);
            statusVfxCo = null;
        }

        spriteRenderer.material = originalMaterial;
        spriteRenderer.color = Color.white;
    }

    private IEnumerator PlayOnDamageVFXCo()
    {
        spriteRenderer.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVFXDuration);
        spriteRenderer.material = originalMaterial;
    }

    private IEnumerator PlayStatusVfxColor(float duration, Color color)
    {
        float tickInterval = .25f;
        float timer = 0;

        Color lightColor = color * 1.2f;
        Color darkColor = color * .8f;
        bool toggle = false;

        while (timer < duration)
        {
            spriteRenderer.color = toggle ? lightColor : darkColor;
            toggle = !toggle;
            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
        }

        spriteRenderer.color = Color.white;
    }
}