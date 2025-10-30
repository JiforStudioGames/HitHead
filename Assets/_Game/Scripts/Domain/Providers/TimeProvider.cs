using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;
using UnityEngine;

namespace Game.Scripts.Domain.Providers
{
    [Serializable]
    public class TimeProvider : EffectProvider
    {
        private static float _originalValue;
        
        protected override void Initialize(EffectsDataModel effectsDataModel)
        {
            _originalValue = Time.timeScale;
        }
        
        public override UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel)
        {
            base.Play(data, effectsDataModel);

            Time.timeScale = _originalValue;
            DOTween.Kill(this);
            DOTween.To(
                    () => 0f,
                    t => Time.timeScale =_originalValue * data.AnimCurve.Evaluate(t),
                    1f,
                    data.Duration
                )
                .OnComplete(() =>
                {
                    Time.timeScale = _originalValue;
                })
                .SetEase(data.AnimProgress)
                .SetId(this);

            return UniTask.Delay(TimeSpan.FromSeconds(data.Duration));
        }
    }
}