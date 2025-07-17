using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Services
{
    public class InventoryService : IInventoryService, IInitializable
    {
        private const string SAVE_KEY = "InventoryData";

        [Inject] private readonly InventoryModel _inventoryModel;
        [Inject] private readonly ShopConfig _shopConfig;

        public IReadOnlyReactiveCollection<WeaponConfig> OwnedWeapons => _inventoryModel.OwnedWeapons;
        public IReadOnlyReactiveProperty<WeaponConfig> SelectedWeapon => _inventoryModel.SelectedWeapon;

        public void Initialize()
        {
            Load();
        }

        public bool AddWeapon(WeaponConfig weapon)
        {
            _inventoryModel.OwnedWeapons.Add(weapon);
            SelectWeapon(weapon);
            Save();
            return true;
        }

        public void SelectWeapon(WeaponConfig weapon)
        {
            if (_inventoryModel.OwnedWeapons.Contains(weapon))
            {
                _inventoryModel.SelectedWeapon.Value = weapon;
                Save();
            }
        }

        public void Load()
        {
            var savedData = PlayerPrefs.GetString(SAVE_KEY, null);
            if (string.IsNullOrEmpty(savedData))
            {
                _inventoryModel.OwnedWeapons.Add(_shopConfig.StartWeapon);
                _inventoryModel.SelectedWeapon.Value = _shopConfig.StartWeapon;
                return;
            }
                

            var savedIds = JsonUtility.FromJson<SaveData>(savedData);
            List<WeaponConfig> allWeapons = _shopConfig.GetAllWeapons();
            foreach (var id in savedIds.OwnedIds)
            {
                var weapon = allWeapons.FirstOrDefault(weapon => weapon.Name == id);
                if (weapon != null) _inventoryModel.OwnedWeapons.Add(weapon);
            }

            WeaponConfig selected = allWeapons.FirstOrDefault(weapon => weapon.Name == savedIds.SelectedId);
            if (selected != null)
                _inventoryModel.SelectedWeapon.Value = selected;
        }

        public void Save()
        {
            var data = new SaveData
            {
                OwnedIds = _inventoryModel.OwnedWeapons.Select(weapon => weapon.Name).ToArray(),
                SelectedId = _inventoryModel.SelectedWeapon.Value?.Name
            };
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        [Serializable]
        private class SaveData
        {
            public string[] OwnedIds;
            public string SelectedId;
        }
    }
}