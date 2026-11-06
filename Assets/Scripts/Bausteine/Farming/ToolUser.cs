using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Benutzt den Gegenstand in der Hand (Hotbar) auf das Ziel vor dem Spieler.
    /// Verbraucht Ausdauer, wenn ein Werkzeug erfolgreich benutzt wurde.
    /// </summary>
    public class ToolUser : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private FacingDirection facing;
        [SerializeField] private Hotbar hotbar;
        [SerializeField] private Stamina stamina;

        [SerializeField] private float reachOffset = 0.7f;
        [SerializeField] private float radius = 0.3f;
        [SerializeField] private LayerMask targetLayers = ~0;

        private readonly List<Collider2D> results = new List<Collider2D>();
        private ContactFilter2D filter;

        /// <summary>Gegenstand wurde benutzt (Animation "Hacken", "Gießen"...).</summary>
        public event Action<ItemData> ItemUsed;

        private void Awake()
        {
            filter = new ContactFilter2D { useTriggers = true };
            filter.SetLayerMask(targetLayers);
        }

        private void Update()
        {
            if (!input.UseToolPressed) return;

            ItemData item = hotbar.SelectedItem;
            if (item == null) return;

            // Erst Ausdauer prüfen, aber erst nach Erfolg abziehen.
            float cost = item is ToolData tool ? tool.StaminaCost : 0f;
            if (stamina != null && stamina.Current < cost) return;

            IToolTarget target = FindTarget();
            if (target == null || !target.UseItem(item, gameObject)) return;

            if (stamina != null) stamina.TryUse(cost);
            ItemUsed?.Invoke(item);
        }

        // Nächstes IToolTarget vor dem Spieler (Raster-Spiele: Radius klein halten!).
        private IToolTarget FindTarget()
        {
            Vector2 point = (Vector2)transform.position + facing.Cardinal * reachOffset;
            Physics2D.OverlapCircle(point, radius, filter, results);

            IToolTarget best = null;
            float bestDistance = float.MaxValue;
            foreach (Collider2D col in results)
            {
                IToolTarget candidate = col.GetComponentInParent<IToolTarget>();
                if (candidate == null) continue;

                float distance = ((Vector2)col.transform.position - point).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = candidate;
                }
            }
            return best;
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 dir = facing != null && Application.isPlaying ? facing.Cardinal : Vector2.down;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)transform.position + dir * reachOffset, radius);
        }
    }
}
