using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;
using Game.Scripts.Services;
using Game.Scripts.UI.Core;
using UniRx;
using Zenject;

namespace Game.Scripts.UI.Shop
{
    public class ShopPresenter : UIScreenPresenter<ShopView>
    {
        [Inject] private readonly ShopConfig _shopConfig;
        
        [Inject] private readonly IInventoryService _inventory;
        [Inject] private readonly ICurrencyService _currency;
        [Inject] private readonly ILevelProgressService _levelProgressService;
        
        private EShopCategory _currentCategory;
        private WeaponConfig _selectedConfig;
        private WeaponSlotView _selectedSlotView;
        private WeaponSlotView _defaultSlotView;
            
        protected override UniTask BeforeShow(CompositeDisposable disposables)
        {
            SpawnCategories(_shopConfig, disposables);
            
            return UniTask.CompletedTask;
        }

        private void SpawnCategories(ShopConfig shopConfig, CompositeDisposable disposables)
        {
            _view.ClearCategories();
            foreach (var category in shopConfig.Categories)
            {
                WeaponCategoryButtonView spawnedCategory = _view.SpawnCategory(category.Key);
                spawnedCategory.OnCategoryToggleClick
                    .Subscribe(isOn =>
                    {
                        spawnedCategory.SetCategoryActive(isOn);
                        if (isOn)
                        {
                            _currentCategory = category.Key;
                            SpawnSlots(category.Value, disposables);
                        }
                    })
                    .AddTo(disposables);
            }
        }

        private void SpawnSlots(ShopConfig.CategoryData categoryWeapons, CompositeDisposable disposables)
        {
            _view.ClearSlots();
            _selectedSlotView = null;
            foreach (var config in categoryWeapons.WeaponSlots)
            {
                WeaponSlotView slot = _view.SpawnWeaponSlot(config);
                slot.SetupSlot(config);
                UpdateSlotState(slot, config);
                slot.OnSlotSelectClick
                    .Subscribe(isOn =>
                    {
                        OnWeaponSlotClicked(slot, config);
                        _view.SetupWeaponInfo(config);
                        slot.SetCategoryActive();
                        if (isOn)
                        {
                        }
                    })
                    .AddTo(disposables);
            }
        }
        
        private void UpdateSlotState(WeaponSlotView slot, WeaponConfig config)
        {
            if (_levelProgressService.CurrentLevel.Value < config.LevelRequired)
                slot.SetState(EWeaponSlotState.LockedByLevel);
            else if (/* TODO: VIP check */ config.VIPRequired)
                slot.SetState(EWeaponSlotState.LockedByVIP);
            else if (!_inventory.OwnedWeapons.Contains(config))
                slot.SetState(_currency.GetBalanceByCurrencyType(config.CurrencyPriceType) >= config.Price
                    ? EWeaponSlotState.AvailableForPurchase
                    : EWeaponSlotState.LockedByCurrency);
            else if (_inventory.SelectedWeapon.Value == config)
            {
                _selectedSlotView?.SetState(EWeaponSlotState.Unselected);
                slot.SetState(EWeaponSlotState.Selected);
                _selectedSlotView = slot;
                _selectedConfig = config;
                _view.SetupWeaponInfo(config);
            }
            else
                slot.SetState(EWeaponSlotState.Unselected);
        }
        
        private void OnWeaponSlotClicked(WeaponSlotView slot, WeaponConfig config)
        {
            if (_levelProgressService.CurrentLevel.Value < config.LevelRequired) 
                return;
            if (!_inventory.OwnedWeapons.Contains(config))
            {
                if (_currency.CanPurchaseByCurrencyType(config.CurrencyPriceType, config.Price))
                {
                    _currency.PurchaseByCurrencyType(config.CurrencyPriceType, config.Price);
                    _inventory.AddWeapon(config);
                    UpdateSlotState(slot, config);
                }
            }
            else
            {
                _inventory.SelectWeapon(config);
                UpdateSlotState(slot, config);
            }
        }
    }
}