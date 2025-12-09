using UnityEngine;
using System.Collections.Generic;

public class Spikes : PoolBase
{
    [Header("Настройки шипов")]
    [SerializeField] private int spikeDamage = 20;
    [SerializeField] private float damageInterval = 1f; 

    [Header("Визуальные эффекты шипов")]
    [SerializeField] private GameObject spikeEffectPrefab;
    [SerializeField] private AudioClip spikeSound;

    private Dictionary<Enemy, Coroutine> activeEnemies = new Dictionary<Enemy, Coroutine>();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (spikeSound != null)
        {
            audioSource.clip = spikeSound;
            audioSource.spatialBlend = 1f;
            audioSource.volume = 0.4f;
        }
    }

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
                Coroutine damageCoroutine = StartCoroutine(ApplySpikeDamage(enemy));
                activeEnemies[enemy] = damageCoroutine;

                if (spikeEffectPrefab != null)
                {
                    //GameObject effect = Instantiate(
                    //    spikeEffectPrefab,
                    //    enemy.transform.position + Vector3.up * 0.5f,
                    //    Quaternion.identity
                    //);
                    //Destroy(effect, 1f);
                }
            }
        }
    }

    protected override void OnEnemyEnterEffect(Enemy enemy)
    {
        if (!enemy.IsAlive || activeEnemies.ContainsKey(enemy)) return;

        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();

        enemy.TakeDamage(spikeDamage / 2);

        Debug.Log($"Враг {enemy.name} наступил на шипы, урон: {spikeDamage / 2}");

        Coroutine damageCoroutine = StartCoroutine(ApplySpikeDamage(enemy));
        activeEnemies[enemy] = damageCoroutine;
    }

    protected override void OnEnemyExitEffect(Enemy enemy)
    {
        if (activeEnemies.ContainsKey(enemy))
        {
            Debug.Log($"Враг {enemy.name} сошел с шипов");

            StopCoroutine(activeEnemies[enemy]);
            activeEnemies.Remove(enemy);

            if (activeEnemies.Count == 0 && audioSource != null)
                audioSource.Stop();
        }
    }

    private System.Collections.IEnumerator ApplySpikeDamage(Enemy enemy)
    {
        yield return new WaitForSeconds(damageInterval * 0.5f);

        while (enemy != null && enemy.IsAlive && isActive)
        {
            enemy.TakeDamage(spikeDamage);

            ShowSpikeEffect(enemy.transform.position);

            yield return new WaitForSeconds(damageInterval);
        }

        if (enemy != null && activeEnemies.ContainsKey(enemy))
        {
            activeEnemies.Remove(enemy);

            if (activeEnemies.Count == 0 && audioSource != null)
                audioSource.Stop();
        }
    }

    private void ShowSpikeEffect(Vector3 position)
    {
        if (spikeEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                spikeEffectPrefab,
                position + Vector3.up * 0.2f,
                Quaternion.identity
            );
            Destroy(effect, 0.5f);
        }
    }

    protected override void UpdateVisualLifetimeIndicator()
    {
        if (poolRenderer == null) return;

        float lifetimePercent = currentLifetime / lifetime;

        Color color = poolRenderer.material.color;
        color.a = Mathf.Lerp(0.2f, 0.8f, lifetimePercent);
        poolRenderer.material.color = color;

        if (currentLifetime < 2f)
        {
            float pulse = Mathf.PingPong(Time.time * 3f, 0.1f);
            transform.localScale = Vector3.one * (1f + pulse);
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
    }
}