using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private Transform _enemySpawnPoint;
        
        public override void InstallBindings()
        {
            Container
                .Bind<Transform>()
                .WithId("EnemySpawnPoint")
                .FromInstance(_enemySpawnPoint)
                .AsSingle();
        }
    }
}