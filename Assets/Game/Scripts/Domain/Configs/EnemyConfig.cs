using System;
using AYellowpaper.SerializedCollections;
using Game.Scripts.Domain.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.Configs
{
    [CreateAssetMenu(menuName = "Enemy/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        public string Name;
        public double Health;
        public float RespawnDelay;
        
        public EnemyComponent Enemy;
    }
}