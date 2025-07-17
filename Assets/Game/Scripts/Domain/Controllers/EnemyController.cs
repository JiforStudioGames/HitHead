using System;
using Game.Scripts.Domain.Components;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Systems;
using Game.Scripts.Services;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Domain.Controllers
{
    public class EnemyController : IEnemyController, IInitializable, IDisposable
    {
        [Inject] private readonly IEffectController _effectController;
        [Inject] private readonly IRewardService _rewardService;
        [Inject] private readonly ILevelProgressService _levelProgress;
        [Inject] private readonly LevelsConfig _levelsConfig;
        [Inject(Id = "EnemySpawnPoint")] private readonly Transform _spawnPoint;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private EnemyComponent _currentEnemy;
        private Health _health;
        
        public void Initialize()
        {
            SpawnEnemy();
        }

        public void Hit(int damage, bool isCrit, Vector3 point, EForceType forceType, EEffectType3D effectType)
        {
            _effectController.PlayEffectGlobal(isCrit ? EEffectType.CritHit : EEffectType.Hit);
            _effectController.PlayEffect3D(effectType, point, _currentEnemy.TargetTransform);
            
            _health.TakeDamage(damage);
            _rewardService.GrantHitReward(damage);

            if (forceType != EForceType.None)
            {
                var force = (point - _currentEnemy.TargetTransform.position).normalized * (int)forceType;
                _currentEnemy.TargetRigidbody.AddForceAtPosition(-force, point, ForceMode.Impulse);
            }
        }
        
        private void HandleDeath()
        {
            _effectController.PlayEffectGlobal(EEffectType.EnemyDeath);
            _effectController.PlayEffect3D(EEffectType3D.DeathEnemy, _currentEnemy.TargetTransform.position);
            _rewardService.GrantKillReward();
            
            _currentEnemy.DestroyTargetObject();
            
            int lvl = _levelProgress.CurrentLevel.Value;
            EnemyConfig enemyConfig = _levelsConfig.Levels[lvl].EnemyOnLevel;
            
            Observable.Timer(TimeSpan.FromSeconds(enemyConfig.RespawnDelay))
                .Subscribe(__ =>
                {
                    Object.Destroy(_currentEnemy.gameObject);
                    SpawnEnemy();
                })
                .AddTo(_disposables);
        }
        
        private void SpawnEnemy()
        {
            int lvl = _levelProgress.CurrentLevel.Value;
            EnemyConfig enemyConfig = _levelsConfig.Levels[lvl].EnemyOnLevel;
            
            _health = new Health(enemyConfig.Health);
            _health.OnDeath
                .Subscribe(_ => HandleDeath())
                .AddTo(_disposables);
            
            _currentEnemy = Object.Instantiate(enemyConfig.Enemy, _spawnPoint.position, Quaternion.identity);
            
            _effectController.PlayEffect3D(EEffectType3D.RespawnEnemy, _currentEnemy.TargetTransform.position);
            _effectController.PlayEffectGlobal(EEffectType.EnemySpawn);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}