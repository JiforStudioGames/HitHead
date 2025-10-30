using System;
using DG.Tweening;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Providers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Game.Scripts.Domain.Configs
{
    [Serializable]
    public struct GlobalEffectData
    { 
        [SerializeReference, SubclassSelector]
        public IEffectProvider Provider;
        public AnimationCurve AnimCurve;
        public Ease AnimProgress;
        public float DelayBeforeEffect;
        public float Duration;
    }
    
    [CreateAssetMenu(menuName = "Effects/EffectsConfig", fileName = "EffectsConfig")]
    public class EffectConfig : ScriptableObject
    {
        public AYellowpaper.SerializedCollections.SerializedDictionary<EEffectType3D, ParticleSystem> Effects3D;
        public AYellowpaper.SerializedCollections.SerializedDictionary<EEffectType, GlobalEffectData[]> EffectsGlobal;
    }
}