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
    public class VignetteProvider : EffectProvider
    {
        [SerializeField] private Color _color;
        
        private static Vignette _vignette;
        private static Color _originalColor;
        
        protected override void Initialize(EffectsDataModel effectsDataModel)
        {
            if (!effectsDataModel.GlobalVolume.profile.TryGet<Vignette>(out var vignette))
                Debug.LogError("[InitializeProvider] Vignette profile is not found");
            
            _vignette = vignette;
            _originalColor = _vignette.color.value;
        }

        public override UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel)
        {
            base.Play(data, effectsDataModel);
            
            _vignette.color.value = _originalColor;
            DOTween.Kill(this);
            DOTween.To(
                    () => _vignette.color.value,
                    c => _vignette.color.value = c,
                    _color,
                    data.Duration
                )
                .OnComplete(() =>
                {
                    _vignette.color.value = _originalColor;
                })
                .SetEase(data.AnimCurve)
                .SetId(this);
            
            return UniTask.Delay(TimeSpan.FromSeconds(data.Duration));
        }
    }
}