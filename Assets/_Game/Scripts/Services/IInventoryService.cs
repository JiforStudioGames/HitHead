using System;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;
using UniRx;

namespace Game.Scripts.Services
{
    public interface IInventoryService
    {
        IReadOnlyReactiveCollection<WeaponConfig> OwnedWeapons { get; }
        IReadOnlyReactiveProperty<WeaponConfig> SelectedWeapon { get; }
        bool AddWeapon(WeaponConfig weapon);
        void SelectWeapon(WeaponConfig weapon);
        void Load();
        void Save();
    }
}