using System;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// "Augen" eines Gegners: Erkennt den Spieler in einem Radius, optional nur mit Sichtlinie.
    /// Wird der Gegner angegriffen, bemerkt er den Angreifer sofort (auch von hinten).
    /// Zwei Radien verhindern Flackern: Erkennen bei detectionRadius, Verlieren erst bei loseRadius.
    /// </summary>
    public class TargetDetector : MonoBehaviour
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float loseRadius = 8f;

        [Header("Sichtlinie")]
        [SerializeField] private bool requireLineOfSight = true;
        [Tooltip("Was die Sicht blockiert (Wände, Felsen...).")]
        [SerializeField] private LayerMask obstacleMask;

        [Tooltip("Wie oft pro Sekunde geprüft wird. Spart Leistung bei vielen Gegnern.")]
        [SerializeField] private float checksPerSecond = 5f;

        private Transform candidate;
        private Health candidateHealth;
        private Health ownHealth;
        private float nextCheckTime;

        /// <summary>Das aktuelle Ziel oder null.</summary>
        public Transform Target { get; private set; }
        public bool HasTarget => Target != null;

        public float DistanceToTarget => HasTarget ? Vector2.Distance(transform.position, Target.position) : float.MaxValue;
        public Vector2 DirectionToTarget => HasTarget ? ((Vector2)(Target.position - transform.position)).normalized : Vector2.zero;

        /// <summary>Ziel erkannt (true) oder verloren (false). Ideal für "!"-Symbol oder Aggro-Sound.</summary>
        public event Action<bool> TargetStatusChanged;

        private void Awake() => ownHealth = GetComponent<Health>();

        private void OnEnable()
        {
            if (ownHealth != null) ownHealth.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            if (ownHealth != null) ownHealth.Damaged -= OnDamaged;
        }

        private void Start()
        {
            GameObject found = GameObject.FindGameObjectWithTag(targetTag);
            if (found == null) return;
            candidate = found.transform;
            candidateHealth = found.GetComponent<Health>();
        }

        private void Update()
        {
            if (Time.time < nextCheckTime || candidate == null) return;
            nextCheckTime = Time.time + 1f / checksPerSecond;

            bool candidateAlive = candidateHealth == null || candidateHealth.IsAlive;
            float distance = Vector2.Distance(transform.position, candidate.position);

            if (!HasTarget)
            {
                if (candidateAlive && distance <= detectionRadius && HasLineOfSight()) SetTarget(candidate);
            }
            else if (!candidateAlive || distance > loseRadius)
            {
                SetTarget(null);
            }
        }

        private bool HasLineOfSight()
        {
            if (!requireLineOfSight) return true;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, candidate.position, obstacleMask);
            return hit.collider == null;
        }

        // Wer zuschlägt, wird bemerkt.
        private void OnDamaged(DamageInfo info)
        {
            if (!HasTarget && candidate != null && info.Source != null &&
                info.Source.transform.root == candidate.root)
            {
                SetTarget(candidate);
            }
        }

        private void SetTarget(Transform newTarget)
        {
            if (Target == newTarget) return;
            Target = newTarget;
            TargetStatusChanged?.Invoke(HasTarget);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(transform.position, loseRadius);
        }
    }
}
