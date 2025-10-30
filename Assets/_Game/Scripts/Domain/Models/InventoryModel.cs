using Game.Scripts.Domain.Configs;
using UniRx;

namespace Game.Scripts.Domain.Models
{
    public class InventoryModel
    {
        /// <summary>
        /// Owned Weapons
        /// </summary>
        public ReactiveCollection<WeaponConfig> OwnedWeapons { get; } = new ReactiveCollection<WeaponConfig>();

        /// <summary>
        /// Current Weapon
        /// </summary>
        public ReactiveProperty<WeaponConfig> SelectedWeapon { get; } = new ReactiveProperty<WeaponConfig>();
    }
}