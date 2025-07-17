using Game.Scripts.Domain.Enums;
using UnityEngine;

namespace Game.Scripts.Domain.Controllers
{
    public interface IEffectController
    {
        void PlayEffect3D(EEffectType3D effectType, Vector3 point, Transform parent = null);
        void PlayEffect3D(ParticleSystem particleSystem, Vector3 point, Transform parent = null);
        void PlayEffectGlobal(EEffectType effectType);
    }
}