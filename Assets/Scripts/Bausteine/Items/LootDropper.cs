using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Lässt beim Tod Beute aus einer LootTable fallen.
    /// Magic Find und Gold Find des Töters (CharacterStats) werden berücksichtigt.
    /// Funktioniert für Gegner genauso wie für Krüge und Büsche.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class LootDropper : MonoBehaviour
    {
        [SerializeField] private LootTable lootTable;

        private Health health;

        private void Awake() => health = GetComponent<Health>();
        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void OnDied(DamageInfo info)
        {
            if (lootTable == null || PickupSpawner.Instance == null) return;

            // Boni des Töters auslesen (z.B. Spieler mit Magic-Find-Ring).
            float magicFind = 0f, goldFind = 0f;
            CharacterStats killerStats = info.Source != null ? info.Source.GetComponentInParent<CharacterStats>() : null;
            if (killerStats != null)
            {
                magicFind = killerStats.GetValue(StatType.MagicFind);
                goldFind = killerStats.GetValue(StatType.GoldFind);
            }

            Vector2 position = transform.position;
            foreach (ItemStack stack in lootTable.RollItems(magicFind))
                PickupSpawner.Instance.SpawnItem(stack, position);

            PickupSpawner.Instance.SpawnGold(lootTable.RollGold(goldFind), position);
        }
    }
}
