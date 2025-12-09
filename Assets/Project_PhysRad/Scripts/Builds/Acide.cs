using System;
using System.Collections.Generic;
using UnityEngine;

public class Acide : PoolBase
{
    [Header("Настройки кислоты")]
    [SerializeField] private int damagePerTick = 5;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private bool damageStacks = false;
    [SerializeField] private int maxDamageStacks = 3;

    [Header("Визуальные эффекты кислоты")]
    [SerializeField] private Material acidMaterial;
    [SerializeField] private GameObject acidBubbleEffect;
    [SerializeField] private float bubbleSpawnRate = 0.5f;

    private Dictionary<Enemy, Coroutine> activeEnemies = new Dictionary<Enemy, Coroutine>();
    private List<IDamageable> damagedTargets = new List<IDamageable>();

    protected override void ApplyEffectToTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            effectRadius,
            affectedLayers
        );

        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && enemy.IsAlive && !activeEnemies.ContainsKey(enemy))
            {
                Coroutine damageCoroutine = StartCoroutine(ApplyAcidDamage(enemy));
                activeEnemies[enemy] = damageCoroutine;
            }
        }

        if (acidBubbleEffect != null && UnityEngine.Random.value < bubbleSpawnRate * Time.deltaTime)
        {
            Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * effectRadius * 0.8f;
            randomPos.y = transform.position.y;
            GameObject bubble = Instantiate(acidBubbleEffect, randomPos, Quaternion.identity);
            Destroy(bubble, 2f);
        }
    }

    protected override void OnEnemyEnterEffect(Enemy enemy)
    {
        if (!enemy.IsAlive || activeEnemies.ContainsKey(enemy)) return;

        Debug.Log($"Враг {enemy.name} вошел в кислотную лужу");

        ApplyAcidVisualEffect(enemy, true);

        Coroutine damageCoroutine = StartCoroutine(ApplyAcidDamage(enemy));
        activeEnemies[enemy] = damageCoroutine;
    }

    protected override void OnEnemyExitEffect(Enemy enemy)
    {
        if (activeEnemies.ContainsKey(enemy))
        {
            Debug.Log($"Враг {enemy.name} вышел из кислотной лужи");

            StopCoroutine(activeEnemies[enemy]);
            activeEnemies.Remove(enemy);

            ApplyAcidVisualEffect(enemy, false);
        }
    }

    private System.Collections.IEnumerator ApplyAcidDamage(Enemy enemy)
    {
        while (enemy != null && isActive)
        {
            enemy.TakeDamage(damagePerTick);

            ShowDamageEffect(enemy.transform.position);

            if (!damagedTargets.Contains(enemy))
                damagedTargets.Add(enemy);

            yield return new WaitForSeconds(damageInterval);
        }

        if (enemy != null && activeEnemies.ContainsKey(enemy))
        {
            activeEnemies.Remove(enemy);
            ApplyAcidVisualEffect(enemy, false);
        }
    }

    private void ApplyAcidVisualEffect(Enemy enemy, bool apply)
    {
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        if (enemyRenderer == null) return;

        if (apply)
        {

            if (!enemyRenderer.material.name.Contains("Acid"))
            {
                enemyRenderer.material = new Material(acidMaterial);
            }
        }
        else
        {
            enemyRenderer.material.color = Color.white;
        }
    }

    private void ShowDamageEffect(Vector3 position)
    {

    }

    protected override void UpdateVisualLifetimeIndicator()
    {
        base.UpdateVisualLifetimeIndicator();

        if (poolRenderer != null)
        {
            float lifetimePercent = currentLifetime / lifetime;

            Color acidColor = Color.Lerp(
                new Color(0, 0.3f, 0, 0.5f),
                new Color(0, 1f, 0.2f, 0.8f),
                lifetimePercent
            );
            poolRenderer.material.color = acidColor;
        }
    }

    void OnDestroy()
    {
        foreach (var coroutine in activeEnemies.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        activeEnemies.Clear();

        foreach (var enemy in activeEnemies.Keys)
        {
            if (enemy != null)
                ApplyAcidVisualEffect(enemy, false);
        }
    }
}
