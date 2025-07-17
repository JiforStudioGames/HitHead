using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.Domain.Providers
{
    [Serializable]
    public class LensDistortionProvider : EffectProvider
    {
        private static LensDistortion _lensDistortion;
        private static float _originalValue;
        
        protected override void Initialize(EffectsDataModel effectsDataModel)
        {
            if (!effectsDataModel.GlobalVolume.profile.TryGet<LensDistortion>(out var lens))
                Debug.LogError("[InitializeProvider] LensDistortion profile is not found");
            
            _lensDistortion = lens;
            _originalValue = _lensDistortion.intensity.value;
        }

        public UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel)
        {
            base.Play(data, effectsDataModel);
            
            DOTween.Kill(this);
            DOTween.To(
                    () => 0f,
                    t => _lensDistortion.intensity.value = _originalValue * data.AnimCurve.Evaluate(t),
                    1f,
                    data.Duration
                )
                .SetEase(data.AnimProgress)
                .SetId(this);

            return UniTask.Delay(TimeSpan.FromSeconds(data.Duration));
        }
    }
}