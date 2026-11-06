using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein Trigger-Collider, der beim Berühren Schaden austeilt (Schwerthieb, Explosion...).
    /// Aktivierung über SetActive(true/false) des GameObjects – jedes Ziel wird pro
    /// Aktivierung nur EINMAL getroffen.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockbackForce = 6f;

        [Tooltip("Wer greift an? Leer = automatisch (TeamMember im Parent). Wird nie selbst getroffen.")]
        [SerializeField] private GameObject owner;

        private readonly HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();
        private TeamMember ownerTeam;

        /// <summary>Schaden pro Treffer. Wird z.B. von Angriffsskripten vor dem Schlag gesetzt.</summary>
        public int Damage { get => damage; set => damage = value; }

        /// <summary>Rückstoßstärke pro Treffer.</summary>
        public float KnockbackForce { get => knockbackForce; set => knockbackForce = value; }

        /// <summary>Markiert den nächsten Treffer als kritisch.</summary>
        public bool IsCritical { get; set; }

        public GameObject Owner => owner;

        /// <summary>Wird bei jedem erfolgreichen Treffer ausgelöst. Parameter = (Ziel, Trefferpunkt).</summary>
        public event Action<IDamageable, Vector2> Hit;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            // Ohne Angabe: Das nächste übergeordnete Objekt mit TeamMember bzw. die Wurzel.
            if (owner == null)
            {
                TeamMember team = GetComponentInParent<TeamMember>();
                owner = team != null ? team.gameObject : transform.root.gameObject;
            }
            ownerTeam = owner.GetComponent<TeamMember>();
        }

        // Jede neue Aktivierung = neuer Schlag = Trefferliste leeren.
        private void OnEnable() => alreadyHit.Clear();

        /// <summary>Erlaubt, dieselben Ziele erneut zu treffen (z.B. bei mehrstufigen Angriffen).</summary>
        public void ResetHits() => alreadyHit.Clear();

        private void OnTriggerEnter2D(Collider2D other) => TryHit(other);

        private void TryHit(Collider2D other)
        {
            // Sich selbst nie treffen.
            if (other.transform.IsChildOf(owner.transform)) return;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null || !target.IsAlive) return;

            // Freund/Feind prüfen.
            TeamMember targetTeam = other.GetComponentInParent<TeamMember>();
            if (!TeamMember.AreHostile(ownerTeam, targetTeam)) return;

            // Jedes Ziel nur einmal pro Aktivierung.
            if (!alreadyHit.Add(target)) return;

            var info = new DamageInfo(damage, owner.transform.position, knockbackForce, owner, IsCritical);
            target.TakeDamage(info);

            Hit?.Invoke(target, other.ClosestPoint(transform.position));
        }
    }
}
