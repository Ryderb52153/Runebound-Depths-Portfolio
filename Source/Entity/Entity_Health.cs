using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, ITake_Damage
{
    [SerializeField] private float currentHealth;

    [Header("Health Settings")]
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenerateHealth = true;

    private Slider healthBar;
    private Entity entity;
    private Entity_Stats entityStats;
    private bool isRegenerating = false;
    protected bool canTakeDamage = true;

    public event Action<Attack_Data> OnTakenDamage;
    public event Action OnHealthUpdated;
    public float lastDamageTaken { get; private set; }
    public bool isDead { get; private set; }

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();

        SetupHealth();
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();
    public float GetMaxHealth() => entityStats.GetMaxHealth();
    public float GetCurrentHealth() => currentHealth;
    public void EnableHealthBar(bool enable) => healthBar?.gameObject.SetActive(enable);

    public virtual void TakeDamage(Attack_Data attackData, KnockbackData knockback)
    {
        if (isDead || !canTakeDamage)
            return;

        if (AttackEvaded())
        {
            UI_FloatingText_Manager.Instance.CreateFloatingText("Evaded", transform.position, TextType.Evaded);
            return;
        }

        float mitigation = entityStats.GetArmorMitigation(attackData.armorPenetration);
        float resistance = entityStats.GetElementalResistance(attackData.element);
        float finalDamage = (attackData.physicalDamage * (1 - mitigation)) +
                            (attackData.elementalDamage * (1 - resistance));

        SoundManager.Instance.PlaySFX(entity.hurtSound, transform.position);
        lastDamageTaken = finalDamage;
        HandleCombatText(attackData);
        ApplyKnockback(attackData.attackerPosition, knockback);
        ReduceHealth(finalDamage, attackData.isCrit);
        OnTakenDamage?.Invoke(attackData);
    }

    private void HandleCombatText(Attack_Data attackData)
    {
        TextType textType = attackData.isCrit ? TextType.Crit : TextType.Damage;
        string text = Mathf.Round(lastDamageTaken).ToString();
        if (attackData.isCrit) text += "!";

        UI_FloatingText_Manager.Instance.CreateFloatingText(text, transform.position, textType);
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private void ApplyKnockback(Vector3 attackerPosition, KnockbackData knockbackData)
    {
        if(entity == null)
            return;

        if (knockbackData == null || !knockbackData.useKnockback)
            return;

        int direction = transform.position.x > attackerPosition.x ? 1 : -1;
        float resistance = entityStats != null ? entityStats.defenseStats.knockbackResistance.GetValue() : 0f;

        Vector2 finalForce = knockbackData.force * (1f - resistance);
        float finalDuration = knockbackData.duration * (1f - resistance);

        finalForce.x *= direction;

        if (finalForce.sqrMagnitude <= 0.0001f || finalDuration <= 0.01f)
            return;

        entity.ReceiveKnockback(finalForce, finalDuration);
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();
        currentHealth = Mathf.Min(newHealth, maxHealth);
        UI_FloatingText_Manager.Instance.CreateFloatingText(Mathf.Round(healAmount).ToString(), transform.position, TextType.Heal);
        UpdateHealthBar();

        if (!isRegenerating)
            CheckForRegeneration();
    }

    public void ReduceHealth(float damage, bool isCrit)
    {
        if(isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0 && !isDead)
            Die();

        UpdateHealthBar();

        if (!isRegenerating)
            CheckForRegeneration();
    }

    private void SetupHealth()
    {
        if (entityStats != null)
        {
            currentHealth = entityStats.GetMaxHealth();
            UpdateHealthBar();
        }
    }

    private void CheckForRegeneration()
    {
        if (!canRegenerateHealth)
            return;

        if (entityStats.GetAttributeValue(Stat_Type.HealthRegen) <= 0)
            return;

        if (isDead)
            return;

        if (currentHealth < entityStats.GetMaxHealth())
            StartCoroutine(RegenerateHealthOverTime());
    }

    private IEnumerator RegenerateHealthOverTime()
    {
        isRegenerating = true;

        while (currentHealth < entityStats.GetMaxHealth() && entityStats.GetAttributeValue(Stat_Type.HealthRegen) > 0)
        {
            RegenerateHealth();
            yield return new WaitForSeconds(regenInterval);
        }
        isRegenerating = false;
    }

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp(percent, 0, 1);
        UpdateHealthBar();
    }

    private void RegenerateHealth()
    {
        float regenAmount = entityStats.GetAttributeValue(Stat_Type.HealthRegen);
        IncreaseHealth(regenAmount);
    }

    protected void UpdatedPersistentStats()
    {
        currentHealth = entityStats.GetMaxHealth();
        SetHealthToPercent(100);
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null || !healthBar.gameObject.activeSelf)
            return;

        healthBar.value = currentHealth / entityStats.GetMaxHealth();
        OnHealthUpdated?.Invoke();
    }

    protected virtual void Die()
    {
        isDead = true;
        SoundManager.Instance.PlaySFX(entity.deathSound, transform.position);
        entity.EntityDeath();
    }

    private bool AttackEvaded()
    {
        if (entityStats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    private void OnEnable()
    {
        isDead = false;
        SetupHealth();
    }
}