using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Game.Scripts.Domain.Enums;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.Configs
{
    [CreateAssetMenu(menuName = "Level/LevelsConfig", fileName = "LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [Serializable]
        public struct LevelData
        {
            [Header("Settings")]
            public int ExpForLevel;
            public EnemyConfig EnemyOnLevel;
            
            [Header("Rewards")]
            public bool AddWeaponForLevel;
            
            [ShowIf("AddWeaponForLevel")] [AllowNesting]
            public WeaponConfig WeaponForLevel;
            
            [Space]
            public bool AddCurrencyForLevel;
            
            [ShowIf("AddCurrencyForLevel")] [AllowNesting]
            public ECurrencyType Currency;
            [BoxGroup("Currency Reward")] [ShowIf("AddCurrencyForLevel")] [AllowNesting]
            public int CurrencyValue;
        }
        
        public SerializedDictionary<int, LevelData> Levels;
    }
}