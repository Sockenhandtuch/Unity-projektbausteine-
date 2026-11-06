using System.Collections.Generic;
using UnityEngine;

namespace Bausteine
{
    /// <summary>
    /// Beschreibt eine Pflanzenart: Wachstumsstufen, Dauer, Ernte, Jahreszeiten.
    /// </summary>
    [CreateAssetMenu(menuName = "Bausteine/Farming/Crop", fileName = "NewCrop")]
    public class CropData : ScriptableObject
    {
        [Tooltip("Ein Sprite pro Wachstumsstufe. Das LETZTE Sprite ist die erntereife Pflanze.")]
        [SerializeField] private Sprite[] stageSprites;
        [SerializeField, Min(1)] private int daysPerStage = 1;

        [Header("Ernte")]
        [SerializeField] private ItemData harvestItem;
        [SerializeField, Min(1)] private int minHarvest = 1;
        [SerializeField, Min(1)] private int maxHarvest = 1;
        [Tooltip("0 = Pflanze verschwindet nach der Ernte. >0 = wächst in so vielen Tagen nach (Tomaten, Beeren).")]
        [SerializeField, Min(0)] private int regrowDays = 0;

        [Header("Jahreszeiten")]
        [SerializeField] private List<Season> growingSeasons = new List<Season> { Season.Spring };

        public ItemData HarvestItem => harvestItem;
        public int RegrowDays => regrowDays;
        public int StageCount => stageSprites != null ? stageSprites.Length : 0;

        /// <summary>Tage vom Säen bis zur Ernte.</summary>
        public int TotalGrowDays => Mathf.Max(0, StageCount - 1) * daysPerStage;

        public bool CanGrowIn(Season season) => growingSeasons.Contains(season);

        /// <summary>Welches Sprite nach so vielen Wachstumstagen angezeigt wird.</summary>
        public Sprite GetSprite(int daysGrown)
        {
            if (StageCount == 0) return null;
            int stage = Mathf.Clamp(daysGrown / daysPerStage, 0, StageCount - 1);
            return stageSprites[stage];
        }

        public int RollHarvestAmount() => Random.Range(minHarvest, maxHarvest + 1);
    }
}
