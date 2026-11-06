using UnityEngine;

namespace Bausteine
{
    /// <summary>Saatgut als Gegenstand – verweist auf die Pflanze, die daraus wächst.</summary>
    [CreateAssetMenu(menuName = "Bausteine/Items/Seed", fileName = "NewSeed")]
    public class SeedData : ItemData
    {
        [SerializeField] private CropData crop;

        public CropData Crop => crop;
    }
}
