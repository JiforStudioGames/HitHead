using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;

namespace Game.Scripts.Domain.Providers
{
    public abstract class EffectProvider : IEffectProvider
    {
        private static readonly HashSet<Type> _initedTypes = new HashSet<Type>();

        protected abstract void Initialize(EffectsDataModel effectsDataModel);

        public virtual UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel)
        {
            var myType = GetType();
            if (!_initedTypes.Contains(myType))
            {
                Initialize(effectsDataModel);
                _initedTypes.Add(myType);
            }
            
            return default;
        }
    }
}