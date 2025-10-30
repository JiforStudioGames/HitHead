using Cysharp.Threading.Tasks;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;

namespace Game.Scripts.Domain.Providers
{
    public interface IEffectProvider
    {
        UniTask Play(GlobalEffectData data, EffectsDataModel effectsDataModel);
    }
}