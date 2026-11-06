using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bausteine
{
    /// <summary>
    /// Sammelt den Zustand aller Spieler-Bausteine in PlayerSaveData und stellt ihn wieder her.
    /// Jeder Baustein ist optional – fehlt z.B. ExperienceSystem, wird er übersprungen.
    /// Hinweis: Die Szene wird hier NICHT geladen. Ein Hauptmenü würde zuerst
    /// data.sceneName laden und danach Load() aufrufen.
    /// </summary>
    public class PlayerSaveController : MonoBehaviour
    {
        [SerializeField] private string saveSlot = "save_1";
        [SerializeField] private ItemDatabase itemDatabase;
        [Tooltip("Beim Start automatisch laden, falls ein Spielstand existiert.")]
        [SerializeField] private bool loadOnStart = false;

        private void Start()
        {
            if (loadOnStart) Load();
        }

        public void Save()
        {
            var data = new PlayerSaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                position = transform.position,
                worldFlags = WorldFlags.GetAll()
            };

            if (TryGetComponent(out PlayerRespawn respawn)) data.respawnPoint = respawn.RespawnPoint;
            if (TryGetComponent(out Health health)) data.health = health.Current;
            if (TryGetComponent(out Wallet wallet)) data.gold = wallet.Amount;
            if (TryGetComponent(out PlayerAbilities abilities)) data.abilities = abilities.GetAll();

            if (TryGetComponent(out ExperienceSystem xp))
            {
                data.level = xp.Level;
                data.xp = xp.CurrentXp;
                data.unspentStatPoints = xp.UnspentStatPoints;
            }

            if (TryGetComponent(out Inventory inventory))
            {
                for (int i = 0; i < inventory.Size; i++)
                {
                    ItemStack slot = inventory.GetSlot(i);
                    data.inventory.Add(new SavedSlot { itemId = slot.Item != null ? slot.Item.Id : null, amount = slot.Amount });
                }
            }

            if (TryGetComponent(out Equipment equipment))
            {
                foreach (EquipSlot slot in System.Enum.GetValues(typeof(EquipSlot)))
                {
                    EquipmentData item = equipment.Get(slot);
                    if (item != null) data.equipment.Add(item.Id);
                }
            }

            if (GameClock.Instance != null)
            {
                data.day = GameClock.Instance.Day;
                data.season = GameClock.Instance.Season;
                data.year = GameClock.Instance.Year;
                data.minuteOfDay = GameClock.Instance.MinuteOfDay;
            }

            SaveSystem.Save(saveSlot, data);
            Debug.Log($"Gespeichert in Slot '{saveSlot}'.");
        }

        public bool Load()
        {
            if (!SaveSystem.TryLoad(saveSlot, out PlayerSaveData data)) return false;

            transform.position = data.position;
            if (TryGetComponent(out Rigidbody2D rb)) rb.position = data.position;
            WorldFlags.LoadAll(data.worldFlags);

            if (TryGetComponent(out PlayerRespawn respawn)) respawn.SetRespawnPoint(data.respawnPoint);
            if (TryGetComponent(out Wallet wallet)) wallet.SetAmount(data.gold);
            if (TryGetComponent(out ExperienceSystem xp)) xp.Restore(data.level, data.xp, data.unspentStatPoints);

            if (TryGetComponent(out PlayerAbilities abilities))
                foreach (AbilityType a in data.abilities) abilities.Unlock(a);

            if (itemDatabase != null)
            {
                if (TryGetComponent(out Inventory inventory))
                {
                    inventory.Clear();
                    for (int i = 0; i < data.inventory.Count && i < inventory.Size; i++)
                    {
                        SavedSlot s = data.inventory[i];
                        inventory.SetSlot(i, itemDatabase.GetById(s.itemId), s.amount);
                    }
                }

                if (TryGetComponent(out Equipment equipment))
                    foreach (string id in data.equipment)
                        equipment.Equip(itemDatabase.GetById(id) as EquipmentData);
            }

            // Leben zuletzt setzen, da Ausrüstung/Level das Maximum verändern können.
            if (TryGetComponent(out Health health)) health.SetCurrent(data.health);

            if (GameClock.Instance != null)
                GameClock.Instance.Restore(data.day, data.season, data.year, data.minuteOfDay);

            if (CameraFollow.Instance != null) CameraFollow.Instance.SnapToTarget();
            return true;
        }
    }
}
