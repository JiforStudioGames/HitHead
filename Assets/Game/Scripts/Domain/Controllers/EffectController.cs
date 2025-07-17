using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Camera;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Domain.Controllers
{
    public class EffectController : IEffectController, IInitializable
    {
        [Inject] private readonly EffectConfig _effectConfig;
        [Inject] private readonly EffectsDataModel _effectsDataModel;
        [Inject] private readonly Cameras _cameras;
        
        public void Initialize()
        {
            
        }

        public void PlayEffect3D(EEffectType3D effectType, Vector3 point, Transform parent = null)
        {
            if (!_effectConfig.Effects3D.ContainsKey(effectType)) 
                return;
            
            PlayEffect3D(_effectConfig.Effects3D[effectType], point, parent);
        }
        
        public void PlayEffect3D(ParticleSystem particleSystem, Vector3 point, Transform parent = null)
        {
            ParticleSystem spawnedEffect = Object.Instantiate(particleSystem, point, Quaternion.identity);
            spawnedEffect.transform.SetParent(parent);
            
            spawnedEffect.Play();
            Object.Destroy(spawnedEffect.gameObject, spawnedEffect.main.duration + spawnedEffect.main.startLifetime.constantMax);
        }

        public void PlayEffectGlobal(EEffectType effectType)
        {
            if (_effectConfig.EffectsGlobal.ContainsKey(effectType))
            {
                foreach (var effectData in _effectConfig.EffectsGlobal[effectType])
                {
                    PlayEffectProvider(effectData);
                }
            }
        }

        // TODO: replace this for script realization 
        private async UniTask PlayEffectProvider(GlobalEffectData effectData)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(effectData.DelayBeforeEffect));

            await effectData.Provider.Play(effectData, _effectsDataModel);
        }
    }
}