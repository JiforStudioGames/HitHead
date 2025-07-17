using System;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
    public class WeaponSlotView : MonoBehaviour
    {
        [Header("Base UI")]
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _activeSelector;
        
        /*[Header("Stats")]
        [SerializeField] private WeaponStatView _statPrefab;*/
        
        [Header("Current State")]
        [SerializeField] private GameObject _stateBody;
        [SerializeField] private TextMeshProUGUI _stateText;
        
        [Header("Price")]
        [SerializeField] private GameObject _priceBody;
        [SerializeField] private Image _currencyIcon;
        [SerializeField] private TextMeshProUGUI _priceText;
        
        [Header("Block")]
        [SerializeField] private GameObject _blockBody;
        [SerializeField] private TextMeshProUGUI _blockInfo;
        
        public IObservable<bool> OnSlotSelectClick => _toggle.onValueChanged.AsObservable();

        private WeaponConfig _weaponConfig;
        private EWeaponSlotState _state;
        
        public void SetupSlot(WeaponConfig config)
        {
            _weaponConfig = config;
            _name.text = config.name;
            _iconImage.sprite = config.Icon;

            _priceText.text = ValueParser.Parse(config.Price);
        }

        public void SetState(EWeaponSlotState state)
        {
            _state = state;
            
            _priceBody.SetActive(state is EWeaponSlotState.AvailableForPurchase or EWeaponSlotState.LockedByCurrency);
            
            _blockBody.SetActive(state is EWeaponSlotState.LockedByLevel or EWeaponSlotState.LockedByVIP);
            _blockInfo.text = GetBlockText(state);
            
            _stateBody.SetActive(state is not (EWeaponSlotState.LockedByCurrency or EWeaponSlotState.AvailableForPurchase));
            _stateText.text = GetStateText(state);

            SetCategoryActive();
        }

        private string GetStateText(EWeaponSlotState state)
        {
            switch (state)
            {
                case EWeaponSlotState.Selected: return "Selected";
                case EWeaponSlotState.Unselected: return "Unselected";
                case EWeaponSlotState.LockedByLevel: return "Locked";
                case EWeaponSlotState.LockedByVIP: return "Locked";
            }
            
            return "Locked";
        }
        
        private string GetBlockText(EWeaponSlotState state)
        {
            switch (state)
            {
                case EWeaponSlotState.LockedByLevel: return $"Level {_weaponConfig.LevelRequired}";
                case EWeaponSlotState.LockedByVIP: return "VIP";
            }
            
            return "Locked";
        }
        
        public void SetCategoryActive()
        {
            _activeSelector.gameObject.SetActive(_state is EWeaponSlotState.Selected);
        }
        
        public void SetToggleGroup(ToggleGroup toggleGroup)
        {
            _toggle.group = toggleGroup;
        }
    }
}