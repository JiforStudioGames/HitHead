using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Controllers;
using Game.Scripts.Domain.Models;
using UnityEngine;
using Zenject;

public class EffectsInstaller : MonoInstaller
{
    [SerializeField] private EffectsDataModel _effectsDataModel;

    public override void InstallBindings()
    {
        Container.BindInstance(_effectsDataModel)
            .AsSingle();
    }
}