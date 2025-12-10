using Pathfinding;
using UnityEngine;
using UnityEngine.Audio;

public class ExplodingEnemy : Enemy
{
    [Header("Взрывные характеристики")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int explosionDamage = 100;
    [SerializeField] private LayerMask explosionTargetLayers;

    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private GameObject warningEffectPrefab;
    [SerializeField] private float warningDuration = 0.5f;

    [Header("Настройки поведения")]
    [SerializeField] private bool explodeOnDeath = false;
    [SerializeField] private bool damageSelf = true;
    [SerializeField] private bool damageOtherEnemies = false;

    private bool isExploding = false;
    private GameObject warningEffect;
    private IAstarAI aiAgent;
    private AudioSource audioSource;

    void Start()
    {
        GameObject obj = Instantiate(m_SpawnParticlePrefab);
        obj.transform.position = transform.position;
        Destroy(obj, 3);

        stationControl = GameController.Instance?.Station;
        audioSource = GetComponent<AudioSource>();

        currentHealth = data.maxHealth;
        aiAgent = GetComponent<IAstarAI>();
        Wall.OnAnyBuildDestroyed += OnAnyBuildDestroyed;
        FindStation();

        if (warningEffectPrefab != null)
        {
            warningEffect = Instantiate(warningEffectPrefab, transform.position, Quaternion.identity, transform);
            warningEffect.SetActive(false);
        }
    }

    public override void Attack(IDamageable target)
    {
        if (isExploding) return;

        StartCoroutine(ExplodeWithWarning());
    }

    public override void Die()
    {
        if (explodeOnDeath && !isExploding)
        {
            Explode();
            return;
        }

        audioSource.PlayOneShot(base.deathSound);

        base.Die();
    }

    private System.Collections.IEnumerator ExplodeWithWarning()
    {
        isExploding = true;

        if (aiAgent != null)
            aiAgent.isStopped = true;

        if (warningEffect != null)
        {
            warningEffect.SetActive(true);

            float elapsedTime = 0f;
            Renderer renderer = GetComponent<Renderer>();
            Color originalColor = renderer.material.color;

            while (elapsedTime < warningDuration)
            {
                float pulse = Mathf.PingPong(elapsedTime * 10f, 1f);
                renderer.material.color = Color.Lerp(originalColor, Color.red, pulse);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            renderer.material.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(warningDuration);
        }

        Explode();
    }
    private void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(explosion, 3f);
        }

        ApplyExplosionDamage();

        if (damageSelf)
        {
            base.Die();
        }
        else
        {
            isExploding = false;
        }
    }

    private void ApplyExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            explosionTargetLayers
        );

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Station") || collider.CompareTag("Build"))
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    float damageMultiplier = 1f - (distance / explosionRadius);
                    int calculatedDamage = Mathf.RoundToInt(explosionDamage * damageMultiplier);

                    damageable.TakeDamage(calculatedDamage);
                    Debug.Log($"Взрыв нанес {calculatedDamage} урона {collider.name}");
                }
            }

        }

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // Дополнительная визуализация для разных расстояний
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, explosionRadius);

        // Зоны разного урона
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius * 0.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isExploding &&
            (collision.gameObject.CompareTag("Station") || collision.gameObject.CompareTag("Build")))
        {
            StopAllCoroutines();
            Explode();
        }
    }

    void Update()
    {
        if (warningEffect != null && warningEffect.activeSelf)
        {
            warningEffect.transform.position = transform.position;
        }
    }

    void OnDestroy()
    {
        if (warningEffect != null)
            Destroy(warningEffect);
    }
}