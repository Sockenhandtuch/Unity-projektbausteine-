using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Ein einzelnes Feld (eine Kachel) zum Anbauen.
    ///
    /// Ablauf: Hacke → umgegraben, Saatgut → gesät, Gießkanne → gegossen,
    /// jeden Morgen wächst gegossene Saat einen Tag. Reif → Interagieren = ernten.
    ///
    /// Aufbau: FarmPlot (SpriteRenderer Boden + BoxCollider2D Trigger) → Kind "Crop" (SpriteRenderer).
    /// </summary>
    public class FarmPlot : MonoBehaviour, IToolTarget, IInteractable
    {
        [Header("Darstellung")]
        [SerializeField] private SpriteRenderer soilRenderer;
        [SerializeField] private SpriteRenderer cropRenderer;
        [SerializeField] private Sprite untilledSprite;
        [SerializeField] private Sprite tilledSprite;
        [SerializeField] private Sprite wateredSprite;

        public bool IsTilled { get; private set; }
        public bool IsWatered { get; private set; }
        public CropData Crop { get; private set; }
        public int DaysGrown { get; private set; }

        public bool HasCrop => Crop != null;
        public bool IsRipe => HasCrop && DaysGrown >= Crop.TotalGrowDays;

        public string Prompt => "Ernten";

        private void Start()
        {
            if (GameClock.Instance != null) GameClock.Instance.DayStarted += OnDayStarted;
            Refresh();
        }

        private void OnDestroy()
        {
            if (GameClock.Instance != null) GameClock.Instance.DayStarted -= OnDayStarted;
        }

        // ---------- Werkzeuge & Saatgut ----------

        public bool UseItem(ItemData item, GameObject user)
        {
            if (item is ToolData tool) return UseTool(tool);
            if (item is SeedData seed) return TryPlant(seed, user);
            return false;
        }

        private bool UseTool(ToolData tool)
        {
            switch (tool.ToolType)
            {
                case ToolType.Hoe:
                    if (IsTilled) return false;
                    IsTilled = true;
                    break;

                case ToolType.WateringCan:
                    if (!IsTilled || IsWatered) return false;
                    IsWatered = true;
                    break;

                case ToolType.Scythe:
                    // Entfernt Pflanzen (z.B. verdorrte) – reife Pflanzen werden lieber geerntet.
                    if (!HasCrop || IsRipe) return false;
                    Crop = null;
                    break;

                default:
                    return false;
            }

            Refresh();
            return true;
        }

        private bool TryPlant(SeedData seed, GameObject user)
        {
            if (!IsTilled || HasCrop || seed.Crop == null) return false;
            if (GameClock.Instance != null && !seed.Crop.CanGrowIn(GameClock.Instance.Season)) return false;

            Inventory inventory = user.GetComponent<Inventory>();
            if (inventory == null || !inventory.Remove(seed)) return false;

            Crop = seed.Crop;
            DaysGrown = 0;
            Refresh();
            return true;
        }

        // ---------- Ernten ----------

        public bool CanInteract(PlayerInteractor interactor) => IsRipe;

        public void Interact(PlayerInteractor interactor)
        {
            if (!IsRipe) return;

            int amount = Crop.RollHarvestAmount();
            Inventory inventory = interactor.GetComponent<Inventory>();
            int leftover = inventory != null ? inventory.Add(Crop.HarvestItem, amount) : amount;
            if (leftover > 0 && PickupSpawner.Instance != null)
                PickupSpawner.Instance.SpawnItem(new ItemStack(Crop.HarvestItem, leftover), transform.position);

            // Nachwachsende Pflanzen springen ein paar Tage zurück, andere verschwinden.
            if (Crop.RegrowDays > 0) DaysGrown = Mathf.Max(0, Crop.TotalGrowDays - Crop.RegrowDays);
            else Crop = null;

            Refresh();
        }

        // ---------- Tageswechsel ----------

        private void OnDayStarted(int day)
        {
            if (HasCrop)
            {
                // Falsche Jahreszeit → Pflanze stirbt.
                if (!Crop.CanGrowIn(GameClock.Instance.Season)) Crop = null;
                else if (IsWatered && !IsRipe) DaysGrown++;
            }

            IsWatered = false; // Jeden Tag neu gießen!
            Refresh();
        }

        /// <summary>Stellt einen gespeicherten Zustand wieder her.</summary>
        public void Restore(bool tilled, bool watered, CropData crop, int daysGrown)
        {
            IsTilled = tilled;
            IsWatered = watered;
            Crop = crop;
            DaysGrown = daysGrown;
            Refresh();
        }

        // ---------- Darstellung ----------

        private void Refresh()
        {
            if (soilRenderer != null)
            {
                soilRenderer.sprite = !IsTilled ? untilledSprite : IsWatered ? wateredSprite : tilledSprite;
            }

            if (cropRenderer != null)
            {
                cropRenderer.enabled = HasCrop;
                if (HasCrop) cropRenderer.sprite = Crop.GetSprite(DaysGrown);
            }
        }
    }
}
