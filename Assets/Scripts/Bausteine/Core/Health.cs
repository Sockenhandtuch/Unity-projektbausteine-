using System;
using UnityEngine;
using UnityEngine.Events;

namespace Bausteine
{
    /// <summary>
    /// Lebenspunkte eines Objekts. Universell nutzbar für Spieler, Gegner und Zerstörbares.
    /// Andere Bausteine (Knockback, Invincibility, LootDropper, UI...) hören auf die Events,
    /// statt dass Health sie direkt kennt. So bleibt alles entkoppelt.
    /// </summary>
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] private int maxHealth = 5;

        [Tooltip("Optional: Werte des Charakters. Wenn gesetzt, reduziert Rüstung den Schaden.")]
        [SerializeField] private CharacterStats stats;

        [Tooltip("Inspector-Event beim Tod (z.B. Sound abspielen, Animation starten).")]
        [SerializeField] private UnityEvent onDied;

        // Zähler statt bool: Mehrere Quellen (Dash, I-Frames, Cutscene) können
        // gleichzeitig Unverwundbarkeit anfordern, ohne sich gegenseitig aufzuheben.
        private int invulnerabilityLocks;

        public int Current { get; private set; }
        public int Max => maxHealth;
        public bool IsAlive => Current > 0;
        public bool IsInvulnerable => invulnerabilityLocks > 0;

        /// <summary>Wird bei jedem erlittenen Treffer ausgelöst (auch beim tödlichen).</summary>
        public event Action<DamageInfo> Damaged;

        /// <summary>Wird bei Heilung ausgelöst. Parameter = tatsächlich geheilte Menge.</summary>
        public event Action<int> Healed;

        /// <summary>Wird bei jeder Änderung ausgelöst. Parameter = (aktuell, maximal). Ideal für UI.</summary>
        public event Action<int, int> Changed;

        /// <summary>Wird genau einmal ausgelöst, wenn das Leben auf 0 fällt.</summary>
        public event Action<DamageInfo> Died;

        /// <summary>Wird ausgelöst, wenn das Objekt wiederbelebt wird.</summary>
        public event Action Revived;

        private void Awake()
        {
            Current = maxHealth;
        }

        /// <summary>Zieht Schaden ab – außer das Objekt ist tot oder unverwundbar.</summary>
        public void TakeDamage(DamageInfo info)
        {
            if (!IsAlive || IsInvulnerable || info.Amount <= 0) return;

            // Rüstung abziehen (falls Werte vorhanden). Das Event erhält den echten Schaden.
            info.Amount = DamageCalculator.ApplyArmor(info.Amount, stats);
            Current = Mathf.Max(0, Current - info.Amount);
            Damaged?.Invoke(info);
            Changed?.Invoke(Current, maxHealth);

            if (Current == 0)
            {
                Died?.Invoke(info);
                onDied?.Invoke();
            }
        }

        /// <summary>Heilt um die angegebene Menge, maximal bis zum Maximum.</summary>
        public void Heal(int amount)
        {
            if (!IsAlive || amount <= 0) return;

            int before = Current;
            Current = Mathf.Min(maxHealth, Current + amount);
            int healed = Current - before;
            if (healed <= 0) return;

            Healed?.Invoke(healed);
            Changed?.Invoke(Current, maxHealth);
        }

        /// <summary>Setzt das Maximum neu (z.B. durch Level-Up oder Herzcontainer).</summary>
        /// <param name="fillUp">true = Leben wird dabei komplett aufgefüllt.</param>
        public void SetMax(int newMax, bool fillUp = false)
        {
            maxHealth = Mathf.Max(1, newMax);
            Current = fillUp ? maxHealth : Mathf.Min(Current, maxHealth);
            Changed?.Invoke(Current, maxHealth);
        }

        /// <summary>Belebt das Objekt mit vollem Leben wieder (z.B. am Checkpoint).</summary>
        public void Revive()
        {
            Current = maxHealth;
            Revived?.Invoke();
            Changed?.Invoke(Current, maxHealth);
        }

        /// <summary>Setzt das Leben direkt (z.B. beim Laden eines Spielstands).</summary>
        public void SetCurrent(int value)
        {
            Current = Mathf.Clamp(value, 0, maxHealth);
            Changed?.Invoke(Current, maxHealth);
        }

        /// <summary>Fordert Unverwundbarkeit an. Muss mit RemoveInvulnerability wieder freigegeben werden.</summary>
        public void AddInvulnerability() => invulnerabilityLocks++;

        /// <summary>Gibt eine Unverwundbarkeits-Anforderung wieder frei.</summary>
        public void RemoveInvulnerability() => invulnerabilityLocks = Mathf.Max(0, invulnerabilityLocks - 1);
    }
}
