using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Game.Scripts.Domain.Enums;
using UnityEngine;

namespace Game.Scripts.Domain.Configs
{
    [CreateAssetMenu(menuName = "Shop/ShopConfig", fileName = "ShopConfig")]
    public class ShopConfig : ScriptableObject
    {
        [Serializable]
        public struct CategoryData
        {
           public List<WeaponConfig> WeaponSlots;
        }

        public WeaponConfig StartWeapon;
        public SerializedDictionary<EShopCategory, CategoryData> Categories;

        public List<WeaponConfig> GetAllWeapons()
        {
            List<WeaponConfig> weaponConfigs = new List<WeaponConfig>();
            foreach (var category in Categories)
            {
                weaponConfigs.AddRange(category.Value.WeaponSlots);
            }

            return weaponConfigs;
        }
    }
}