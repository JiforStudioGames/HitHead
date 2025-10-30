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
    public class ChromaticAberrationProvider : EffectProvider
    {
        [SerializeField][Range(0f,1f)] 
        private float _targetIntensity = 1f;
        
        private static ChromaticAberration _chromaticAberration;
        private static float _originalValue;
        
        protected override void Initialize(EffectsDataModel effectsDataModel)
        {
            if (!effectsDataModel.GlobalVolume.profile.TryGet<ChromaticAberration>(out var ca))
                Debug.LogError("[InitializeProvider] ChromaticAberration profile is not found");

            _chromaticAberration = ca;
            _originalValue = ca.intensity.value;
        }

        public override UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel)
        {
            base.Play(data, effectsDataModel);
            
            _chromaticAberration.intensity.value = _originalValue;
            DOTween.Kill(this);
            DOTween.To(
                    () => 0f,
                    t => _chromaticAberration.intensity.value = Mathf.Lerp(_originalValue, _targetIntensity, data.AnimCurve.Evaluate(t)),
                    1f,
                    data.Duration
                )
                .SetEase(data.AnimProgress)
                .SetId(this)
                .OnComplete(() =>
                {
                    _chromaticAberration.intensity.value = _originalValue;
                });

            return UniTask.Delay(TimeSpan.FromSeconds(data.Duration));
        }
    }
}