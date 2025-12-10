using UnityEngine;
using System.Collections.Generic;

public class Slime : PoolBase
{
    [Header("Настройки слизи")]
    [SerializeField] private float slowPower = 0.5f;
    [SerializeField] private bool slowStacks = false;
    [SerializeField] private float maxSlowPower = 0.2f;

    [Header("Визуальные эффекты слизи")]
    [SerializeField] private Material slimeMaterial;
    [SerializeField] private GameObject slimeTrailEffect;
    [SerializeField] private AudioClip slimeSound;

    private Dictionary<Enemy, float> originalSpeeds = new Dictionary<Enemy, float>();
    private Dictionary<Enemy, int> slowStacksCount = new Dictionary<Enemy, int>();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = slimeSound;
        audioSource.spatialBlend = 1f;
        audioSource.volume = 0.3f;
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
            if (enemy != null && enemy.IsAlive && !originalSpeeds.ContainsKey(enemy))
            {
                ApplySlowEffect(enemy);
            }
        }
    }

    protected override void OnEnemyEnterEffect(Enemy enemy)
    {
        if (!enemy.IsAlive) return;

        Debug.Log($"Враг {enemy.name} вошел в лужу слизи");


        //if (audioSource != null && !audioSource.isPlaying)
        //    audioSource.Play();

        ApplySlimeVisualEffect(enemy, true);

        ApplySlowEffect(enemy);

        if (slimeTrailEffect != null)
        {
            GameObject trail = Instantiate(
                slimeTrailEffect,
                enemy.transform.position,
                Quaternion.identity,
                enemy.transform
            );
            Destroy(trail, 5f);
        }
    }

    protected override void OnEnemyExitEffect(Enemy enemy)
    {
        if (originalSpeeds.ContainsKey(enemy))
        {
            Debug.Log($"Враг {enemy.name} вышел из лужи слизи");
            RemoveSlowEffect(enemy);
            ApplySlimeVisualEffect(enemy, false);
        }
    }

    private void ApplySlowEffect(Enemy enemy)
    {
        var aiPath = enemy.GetComponent<Pathfinding.AIPath>();
        if (aiPath == null) return;

        if (!originalSpeeds.ContainsKey(enemy))
        {
            originalSpeeds[enemy] = aiPath.maxSpeed;
        }

        float currentSpeed = originalSpeeds[enemy];
        float slowedSpeed = currentSpeed * slowPower;

        if (slowStacks && slowStacksCount.ContainsKey(enemy))
        {
            int stacks = slowStacksCount[enemy];
            float stackMultiplier = Mathf.Pow(slowPower, stacks + 1);
            slowedSpeed = currentSpeed * Mathf.Max(stackMultiplier, maxSlowPower);
            slowStacksCount[enemy] = stacks + 1;
        }
        else if (slowStacks)
        {
            slowStacksCount[enemy] = 1;
        }

        aiPath.maxSpeed = slowedSpeed;

        Debug.Log($"Враг замедлен: {originalSpeeds[enemy]:F1} → {slowedSpeed:F1}");
    }

    private void RemoveSlowEffect(Enemy enemy)
    {

        var aiPath = enemy.GetComponent<Pathfinding.AIPath>();
        if (aiPath != null && originalSpeeds.ContainsKey(enemy))
        {
            aiPath.maxSpeed = originalSpeeds[enemy];
            originalSpeeds.Remove(enemy);

            if (slowStacksCount.ContainsKey(enemy))
                slowStacksCount.Remove(enemy);
        }

        if (originalSpeeds.Count == 0 && audioSource != null)
            audioSource.Stop();
    }

    private void ApplySlimeVisualEffect(Enemy enemy, bool apply)
    {
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        if (enemyRenderer == null) return;

        if (apply)
        {
            enemyRenderer.material = new Material(slimeMaterial);

            var slimeParticles = enemy.GetComponentInChildren<ParticleSystem>();
            if (slimeParticles == null)
            {
                GameObject particles = new GameObject("SlimeParticles");
                particles.transform.parent = enemy.transform;
                particles.transform.localPosition = Vector3.up * 0.5f;

                var ps = particles.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = new Color(0, 0.8f, 0, 0.5f);
                main.startSize = 0.1f;
                main.startLifetime = 1f;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
            }
        }
        else
        {

            enemyRenderer.material.color = Color.white;

            var slimeParticles = enemy.GetComponentInChildren<ParticleSystem>();
            if (slimeParticles != null)
                Destroy(slimeParticles.gameObject);
        }
    }

    protected override void UpdateVisualLifetimeIndicator()
    {
        base.UpdateVisualLifetimeIndicator();

        if (poolRenderer != null)
        {
            float lifetimePercent = currentLifetime / lifetime;

            Color slimeColor = Color.Lerp(
                new Color(0, 0.6f, 0, 0.3f),
                new Color(0, 0.3f, 0, 0.7f),
                1f - lifetimePercent
            );
            poolRenderer.material.color = slimeColor;

            poolRenderer.material.SetFloat("_Viscosity", lifetimePercent);
        }
    }

    void OnDestroy()
    {
        foreach (var enemy in originalSpeeds.Keys)
        {
            if (enemy != null)
            {
                RemoveSlowEffect(enemy);
                ApplySlimeVisualEffect(enemy, false);
            }
        }
        originalSpeeds.Clear();
        slowStacksCount.Clear();
    }
}