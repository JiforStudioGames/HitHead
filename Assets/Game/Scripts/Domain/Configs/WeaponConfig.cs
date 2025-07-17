using System;
using System.Globalization;
using AYellowpaper.SerializedCollections;
using Game.Scripts.Domain.Enums;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.Configs
{
    [Serializable]
    public struct Stat
    {
        public EStatTextType TextType;
        public float Value;
    }
    
    [CreateAssetMenu(menuName = "Weapons/WeaponConfig", fileName = "NewWeapon")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Base Settings")]
        public string Name;
        public Sprite Icon;
        public string Description;
        public EEffectType3D EffectForHit;
        
        [Header("Open Condition")]
        public int LevelRequired;
        public bool VIPRequired;
        [FormerlySerializedAs("CurrencyPriceType")] public ECurrencyType CurrencyPriceType;
        public int Price;
        
        [Header("Weapon Settings")]
        public EWeaponType WeaponType;
        public EForceType ForceType;
        public SerializedDictionary<EStatsTypes, Stat> Stats;
        
        [ShowIf("WeaponType", EWeaponType.Melee)]
        public Collider MeleeObject;
        [ShowIf("WeaponType", EWeaponType.Melee)]
        public Vector3 MeleeRotation;
        [ShowIf("WeaponType", EWeaponType.Melee)]
        public float MeleeOffset;
        
        public static string GetParsedStatValue(Stat stat)
        {
            switch (stat.TextType)
            {
                case EStatTextType.Int: return stat.Value.ToString("F0", CultureInfo.InvariantCulture);
                case EStatTextType.Float: return stat.Value.ToString("F1", CultureInfo.InvariantCulture);
                case EStatTextType.IntPercent: return stat.Value.ToString("P0", CultureInfo.InvariantCulture);
                case EStatTextType.FloatPercent: return stat.Value.ToString("P1", CultureInfo.InvariantCulture);
                default: return stat.Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}