using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;

namespace Game.Scripts.Domain.Providers
{
    [Serializable]
    public class CameraZoomProvider : EffectProvider
    {
        private static UnityEngine.Camera _camera;
        private static float _originalFOV;

        protected override void Initialize(EffectsDataModel effectsDataModel)
        {
            _camera = effectsDataModel.MainCamera;
            _originalFOV = _camera.fieldOfView;
        }

        public override UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel)
        {
            base.Play(data, effectsDataModel);

            _camera.fieldOfView = _originalFOV;
            DOTween.Kill(this);
            DOTween.To(
                    () => 0f,
                    t => _camera.fieldOfView = _originalFOV * data.AnimCurve.Evaluate(t),
                    1f,
                    data.Duration
                )
                .SetEase(data.AnimProgress)
                .SetId(this);

            return UniTask.Delay(TimeSpan.FromSeconds(data.Duration));
        }
    }
}