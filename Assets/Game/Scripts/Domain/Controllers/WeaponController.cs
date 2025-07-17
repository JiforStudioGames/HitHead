using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;
using Game.Scripts.Domain.Systems;
using Game.Scripts.Input;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Domain.Controllers
{
    public class WeaponController : IInitializable, IDisposable
    {
        private const float DELAY_MELEE_HITS = 0.05f;

        [Inject] private readonly IEnemyController _enemyController;
        [Inject] private readonly IInputService _input;
        [Inject] private readonly InventoryModel _inventory;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly CompositeDisposable _weaponDisposables = new CompositeDisposable();
        
        public void Initialize()
        {
            _inventory.SelectedWeapon
                .Subscribe(weapon => SetupWeapon(weapon))
                .AddTo(_disposables);
        }
        
        private void SetupWeapon(WeaponConfig weapon)
        {
            _weaponDisposables?.Clear();

            switch (weapon.WeaponType)
            {
                case EWeaponType.Click:
                    HandleClickWeapon(weapon).Forget();
                    break;
                case EWeaponType.Shoot:
                    //HandleShootWeapon(weapon).Forget();
                    break;
                case EWeaponType.Melee:
                    HandleMeleeWeapon(weapon).Forget();
                    break;
                case EWeaponType.Spawn:
                    //HandleSpawnWeapon(weapon).Forget();
                    break;
            }
        }

        private UniTask HandleClickWeapon(WeaponConfig weapon)
        {
            _input.OnTap
                .Subscribe(ray =>
                {
                    //Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 10f);
                    
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.CompareTag("Target"))
                        {
                            (int damage, bool isCrit) = DamageSystem.CalculateDamage(weapon);
                            Debug.Log($"[ClickHit] Damage: {damage}. Critical: {isCrit}");
                            _enemyController.Hit(damage, isCrit, hit.point, weapon.ForceType, weapon.EffectForHit);
                        }
                    }
                })
                .AddTo(_weaponDisposables);

            return UniTask.CompletedTask;
        }
        
        private UniTask HandleMeleeWeapon(WeaponConfig weapon)
        {
            Collider meleeSpawnedObj = null;
            
            _input.OnTap
                .Subscribe(ray =>
                {
                    Vector3 spawnPos = ray.origin + ray.direction * weapon.MeleeOffset;
                    meleeSpawnedObj = Object.Instantiate(weapon.MeleeObject, spawnPos, Quaternion.Euler(weapon.MeleeRotation));
                    
                    meleeSpawnedObj.OnCollisionEnterAsObservable()
                        .Where(col => col.collider.CompareTag("Target"))
                        .ThrottleFirst(TimeSpan.FromSeconds(DELAY_MELEE_HITS))
                        .Subscribe(col =>
                        {
                            Vector3 hitPoint = col.contacts[0].point;
                            
                            (int damage, bool isCrit) = DamageSystem.CalculateDamage(weapon);
                            Debug.Log($"[MeleeHit] Damage: {damage}. Critical: {isCrit}");
                            _enemyController.Hit(damage, isCrit, hitPoint, weapon.ForceType, weapon.EffectForHit);
                        })
                        .AddTo(meleeSpawnedObj);
                })
                .AddTo(_weaponDisposables);
            
            _input.OnDrag
                .Subscribe(ray =>
                {
                    if (meleeSpawnedObj == null) 
                        return;
                    
                    Vector3 pos = ray.origin + ray.direction * weapon.MeleeOffset;
                    meleeSpawnedObj.transform.position = pos;
                })
                .AddTo(_weaponDisposables);
            
            _input.OnRelease
                .Subscribe(_ =>
                {
                    if (meleeSpawnedObj != null)
                    {
                        Object.Destroy(meleeSpawnedObj.gameObject);
                        meleeSpawnedObj = null;
                    }
                })
                .AddTo(_weaponDisposables);
            
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}