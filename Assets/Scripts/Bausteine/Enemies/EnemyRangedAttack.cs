using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Fernkampfangriff: Schießt ein oder mehrere Projektile (Fächer) auf das Ziel.
    /// Mit projectileCount = 5 und spreadAngle = 60 entsteht ein Diablo-artiger Feuerfächer.
    /// </summary>
    public class EnemyRangedAttack : EnemyAttack
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private int baseDamage = 1;

        [Header("Muster")]
        [SerializeField, Min(1)] private int projectileCount = 1;
        [Tooltip("Gesamter Fächerwinkel in Grad (nur bei mehreren Projektilen).")]
        [SerializeField] private float spreadAngle = 30f;

        [Header("Timing")]
        [SerializeField] private float aimTime = 0.5f;
        [SerializeField] private float recoveryTime = 0.3f;

        private CharacterStats stats;
        private Rigidbody2D rb;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
            rb = GetComponent<Rigidbody2D>();
            if (firePoint == null) firePoint = transform;
        }

        protected override IEnumerator PerformAttack(Transform target)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(aimTime);
            if (target == null) yield break;

            Vector2 baseDirection = ((Vector2)(target.position - firePoint.position)).normalized;
            Fire(baseDirection);

            yield return new WaitForSeconds(recoveryTime);
        }

        private void Fire(Vector2 baseDirection)
        {
            float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
            // Bei mehreren Projektilen gleichmäßig über den Fächer verteilen.
            float step = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;
            float startAngle = baseAngle - (projectileCount > 1 ? spreadAngle * 0.5f : 0f);

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = (startAngle + step * i) * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                int damage = DamageCalculator.CalculateOutgoing(baseDamage, stats, out bool crit);
                Projectile p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                p.Launch(dir, damage, gameObject, crit);
            }
        }
    }
}
