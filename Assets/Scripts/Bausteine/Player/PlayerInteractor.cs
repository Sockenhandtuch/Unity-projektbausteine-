using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Sucht das nächstgelegene interagierbare Objekt vor dem Spieler
    /// (Truhe, Schild, NPC, Feld...) und löst die Interaktion per Tastendruck aus.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private FacingDirection facing;

        [Tooltip("Wie weit vor dem Spieler gesucht wird.")]
        [SerializeField] private float reachOffset = 0.5f;
        [SerializeField] private float radius = 0.6f;
        [SerializeField] private LayerMask interactableLayers = ~0;

        private readonly List<Collider2D> results = new List<Collider2D>();
        private ContactFilter2D filter;

        /// <summary>Das aktuell anvisierte Objekt (oder null).</summary>
        public IInteractable Current { get; private set; }

        /// <summary>Ziel hat gewechselt (z.B. um den "E: Öffnen"-Hinweis anzuzeigen).</summary>
        public event Action<IInteractable> TargetChanged;

        private Vector2 SearchPoint => (Vector2)transform.position + facing.Direction * reachOffset;

        private void Awake()
        {
            filter = new ContactFilter2D { useTriggers = true };
            filter.SetLayerMask(interactableLayers);
        }

        private void Update()
        {
            UpdateTarget();

            if (input.InteractPressed && Current != null && Current.CanInteract(this))
                Current.Interact(this);
        }

        private void UpdateTarget()
        {
            IInteractable best = null;
            float bestDistance = float.MaxValue;

            Physics2D.OverlapCircle(SearchPoint, radius, filter, results);
            foreach (Collider2D col in results)
            {
                IInteractable candidate = col.GetComponentInParent<IInteractable>();
                if (candidate == null || !candidate.CanInteract(this)) continue;

                float distance = ((Vector2)col.transform.position - SearchPoint).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = candidate;
                }
            }

            if (best != Current)
            {
                Current = best;
                TargetChanged?.Invoke(Current);
            }
        }

        // Zeigt den Suchbereich im Editor an.
        private void OnDrawGizmosSelected()
        {
            Vector2 dir = facing != null && Application.isPlaying ? facing.Direction : Vector2.down;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere((Vector2)transform.position + dir * reachOffset, radius);
        }
    }
}
