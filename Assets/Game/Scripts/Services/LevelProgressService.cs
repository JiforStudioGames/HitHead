using System;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Services
{
    public class LevelProgressService : ILevelProgressService, IInitializable, IDisposable
    {
        private const string LEVEL_KEY = "CurrentLevel";
        private const string Exp_KEY = "LevelExp";
        
        [Inject] private readonly LevelsConfig _levelsConfig;
        [Inject] private readonly LevelProgressModel _levelProgressModel;
        
        public IReadOnlyReactiveProperty<float> Progress => _levelProgressModel.Progress;
        public IReadOnlyReactiveProperty<int> CurrentLevel => _levelProgressModel.CurrentLevel;
        public IReadOnlyReactiveProperty<float> CurrentExp => _levelProgressModel.CurrentExp;
        public float MaxExpLevel => _levelsConfig.Levels[CurrentLevel.Value].ExpForLevel;
        public int MaxLevel => _levelsConfig.Levels.Count;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        public void Initialize()
        {
            _levelProgressModel.CurrentLevel.Value = PlayerPrefs.GetInt(LEVEL_KEY, 1);
            _levelProgressModel.CurrentExp.Value = PlayerPrefs.GetFloat(Exp_KEY, 0);
            
            CurrentExp
                .Subscribe(exp =>
                {
                    PlayerPrefs.SetFloat(Exp_KEY, exp);
                    _levelProgressModel.Progress.Value = _levelProgressModel.CurrentExp.Value / MaxExpLevel;
                })
                .AddTo(_disposable);
            
            CurrentLevel
                .Subscribe(level => 
                {
                    PlayerPrefs.SetInt(LEVEL_KEY, level); 
                })
                .AddTo(_disposable);
        }
        
        public void AddExp(float value)
        {
            if(_levelProgressModel.CurrentLevel.Value >= MaxLevel)
                return;
            
            _levelProgressModel.CurrentExp.Value += value;

            if (_levelProgressModel.CurrentExp.Value >= MaxExpLevel)
            {
                _levelProgressModel.CurrentExp.Value = 0;
                _levelProgressModel.CurrentLevel.Value++;
                // TODO: reward logic (coins or weapon)
            }
        }

        public void ResetProgress()
        {
            _levelProgressModel.CurrentLevel.Value = 0;
            _levelProgressModel.CurrentExp.Value = 0;
            _levelProgressModel.Progress.Value = 0;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}