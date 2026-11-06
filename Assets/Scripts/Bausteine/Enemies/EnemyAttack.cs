using System;
using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Basisklasse für alle Gegnerangriffe. Kümmert sich um Reichweite, Abklingzeit
    /// und den Zustand "greift gerade an". Unterklassen beschreiben nur noch den Angriff selbst.
    /// Neue Angriffsarten (Sprung, Flächenangriff...) = neue Unterklasse.
    /// </summary>
    public abstract class EnemyAttack : MonoBehaviour
    {
        [SerializeField] protected float range = 1f;
        [SerializeField] protected float cooldown = 1.2f;

        private float readyTime;

        public bool IsAttacking { get; private set; }
        public bool CanAttack => !IsAttacking && Time.time >= readyTime;
        public float Range => range;

        /// <summary>Angriff beginnt. Parameter = Richtung zum Ziel (für Animation).</summary>
        public event Action<Vector2> AttackStarted;

        public bool IsInRange(Transform target)
        {
            return target != null && Vector2.Distance(transform.position, target.position) <= range;
        }

        /// <summary>Startet den Angriff, falls möglich.</summary>
        public void Begin(Transform target)
        {
            if (!CanAttack || target == null) return;
            StartCoroutine(Run(target));
        }

        private IEnumerator Run(Transform target)
        {
            IsAttacking = true;
            AttackStarted?.Invoke(((Vector2)(target.position - transform.position)).normalized);

            yield return PerformAttack(target);

            readyTime = Time.time + cooldown;
            IsAttacking = false;
        }

        /// <summary>Der eigentliche Angriff. Darf über mehrere Frames laufen (Coroutine).</summary>
        protected abstract IEnumerator PerformAttack(Transform target);

        protected virtual void OnDisable()
        {
            IsAttacking = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
