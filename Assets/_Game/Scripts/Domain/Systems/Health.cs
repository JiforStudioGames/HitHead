using System;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Domain.Systems
{
    public class Health
    {
        public ReactiveProperty<double> CurrentHealth { get; }
        public double MaxHealth { get; }
        
        private readonly Subject<Unit> _deathSubject = new Subject<Unit>();
        private bool _isDead;

        public bool IsDead => _isDead;
        public IObservable<Unit> OnDeath => _deathSubject;
        
        public Health(double maxHealth)
        {
            if (maxHealth <= 0)
                throw new ArgumentException("Max health must be greater than zero.");

            MaxHealth = maxHealth;
            CurrentHealth = new ReactiveProperty<double>(maxHealth);
            _isDead = false;
        }
        
        public void TakeDamage(int amount)
        {
            if (IsDead) return;
            if (amount <= 0) return;

            CurrentHealth.Value = Math.Max(CurrentHealth.Value - amount, 0f);

            if (CurrentHealth.Value <= 0f && !_isDead)
            {
                _isDead = true;
                _deathSubject.OnNext(Unit.Default);
                _deathSubject.OnCompleted();
            }
        }
        
        public void Heal(int amount)
        {
            if (_isDead) return;
            if (amount <= 0) return;

            CurrentHealth.Value = Math.Min(CurrentHealth.Value + amount, MaxHealth);
        }
        
        public void FullHeal()
        {
            if (_isDead) return;
            CurrentHealth.Value = MaxHealth;
        }
    }
}