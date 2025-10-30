using System.Collections.Generic;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Models;
using Game.Scripts.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop.WeaponInfo
{
    public class WeaponInfoView : UIScreenView
    {
        [Header("UI Base")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _iconWeapon;
        
        [Header("Stats")]
        [SerializeField] private Transform _statsContent;
        [SerializeField] private WeaponStatView _weaponStatPrefab;

        private List<WeaponStatView> _spawnedStats = new List<WeaponStatView>();

        public void SetupWeaponInfo(WeaponConfig config)
        {
            _nameText.text = config.Name;
            _descriptionText.text = config.Description;
            _iconWeapon.sprite = config.Icon;

            SetupStats(config);
        }

        private void SetupStats(WeaponConfig config)
        {
            ClearStats();
            foreach (var stat in config.Stats)
            {
                WeaponStatView statView = Instantiate(_weaponStatPrefab, _statsContent);
                statView.SetupStat(stat.Key.ToString(), WeaponConfig.GetParsedStatValue(stat.Value));
                _spawnedStats.Add(statView);
            }
        }

        private void ClearStats()
        {
            foreach (var stat in _spawnedStats)
            {
                Destroy(stat.gameObject);
            }
            _spawnedStats.Clear();
        }
    }
}