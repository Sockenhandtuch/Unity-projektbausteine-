using System.Collections;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Abbaubare Ressource: Baum (Axt), Stein/Erz (Spitzhacke), Gras (Sense).
    /// Braucht mehrere Schläge; stärkere Werkzeuge (Power) brauchen weniger.
    /// Wirft beim Abbau Beute aus einer LootTable ab.
    /// </summary>
    public class ResourceNode : MonoBehaviour, IToolTarget
    {
        [SerializeField] private ToolType requiredTool = ToolType.Axe;
        [Tooltip("Mindest-Werkzeugstufe (z.B. Eisenerz braucht Spitzhacke Stufe 2).")]
        [SerializeField, Min(1)] private int requiredPower = 1;
        [SerializeField, Min(1)] private int durability = 3;
        [SerializeField] private LootTable drops;

        [Header("Feedback")]
        [Tooltip("Optional: Grafik-Kindobjekt, das beim Schlag kurz wackelt.")]
        [SerializeField] private Transform visual;
        [SerializeField] private float shakeAmount = 0.05f;
        [SerializeField] private GameObject hitEffectPrefab;

        private int remaining;
        private Vector3 visualOrigin;

        private void Awake()
        {
            remaining = durability;
            if (visual != null) visualOrigin = visual.localPosition;
        }

        public bool UseItem(ItemData item, GameObject user)
        {
            if (!(item is ToolData tool) || tool.ToolType != requiredTool) return false;
            if (tool.Power < requiredPower) return false; // Werkzeug zu schwach

            remaining -= tool.Power;
            if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            if (visual != null) StartCoroutine(Wobble());

            if (remaining <= 0) Break(user);
            return true;
        }

        // Grafik kurz versetzen und wieder zurücksetzen – das Objekt selbst bleibt stehen.
        private IEnumerator Wobble()
        {
            visual.localPosition = visualOrigin + (Vector3)(Random.insideUnitCircle * shakeAmount);
            yield return new WaitForSeconds(0.05f);
            visual.localPosition = visualOrigin;
        }

        private void Break(GameObject user)
        {
            if (drops != null && PickupSpawner.Instance != null)
            {
                foreach (ItemStack stack in drops.RollItems())
                    PickupSpawner.Instance.SpawnItem(stack, transform.position);
            }
            Destroy(gameObject);
        }
    }
}
